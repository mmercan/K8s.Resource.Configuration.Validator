using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sentinel.Core.K8s.Watchers
{

#nullable enable
    public sealed class ResourceWatcher : IDisposable

    {
        private const double MaxRetrySeconds = 32;

        private readonly Subject<(WatchEventType Event, object Resource)> _watchEvents = new();
        private readonly IKubernetesClient _client;
        private readonly ILogger _logger;
        private readonly string _group;
        private readonly string _plural;
        private readonly string _version;

        // private readonly ResourceWatcherMetrics<object> _metrics;
        // private readonly IOptions<OperatorSettings<object>> _settings;
        private readonly Subject<TimeSpan> _reconnectHandler = new();
        private readonly IDisposable _reconnectSubscription;
        private readonly Random _rnd = new();

        private IDisposable? _resetReconnectCounter;
        private int _reconnectAttempts;
        private CancellationTokenSource? _cancellation;
        private Watcher<object>? _watcher;
        private string? Namespace;
        private uint WatcherHttpTimeout;


        public ResourceWatcher(
            IKubernetesClient client,
            ILogger logger,
            string group,
            string plural,
            string version
        )

        //"operator", "kind", "group", "version", "scope"
        //    ResourceWatcherMetrics<TEntity> metrics,
        //    IOptions<OperatorSettings<TEntity>> settings)
        {
            _client = client;
            _logger = logger;

            _group = group;
            _plural = plural;
            _version = version;
            // _kind = kind;
            //     _metrics = metrics;
            //     _settings = settings;

            // if (settings.Value != null)
            // {
            //     this.Namespace = settings.Value.Namespace;
            //     this.WatcherHttpTimeout = settings.Value.WatcherHttpTimeout;

            // }
            // else
            // {
            this.Namespace = null;
            this.WatcherHttpTimeout = 86400; //24 hours
                                             //  }

            _reconnectSubscription =
                _reconnectHandler
                    .Select(Observable.Timer)
                    .Switch()
                    .Subscribe(async _ => await WatchResource());
        }

        public IObservable<(WatchEventType Event, object Resource)> WatchEvents => _watchEvents;

        public Task Start()
        {
            _logger.LogDebug(@"Resource Watcher startup for type ""{type}"".", _plural);
            return WatchResource();
        }

        public Task Stop()
        {
            _logger.LogTrace(@"Resource Watcher shutdown for type ""{type}"".", _plural);
            Disposing(true);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Disposing(false);
        }

        private void Disposing(bool fromStop)
        {
            if (!fromStop)
            {
                _watchEvents.Dispose();
                _reconnectHandler.Dispose();
            }

            _reconnectHandler.Dispose();
            _reconnectSubscription.Dispose();
            if (_cancellation?.IsCancellationRequested == false)
            {
                _cancellation.Cancel();
            }

            _cancellation?.Dispose();
            _watcher?.Dispose();
            _logger.LogTrace(@"Disposed resource watcher for type ""{type}"".", _plural);
            // _metrics.Running.Set(0);
        }

        private async Task WatchResource()
        {
            if (_watcher != null)
            {
                if (!_watcher.Watching)
                {
                    _watcher.Dispose();
                }
                else
                {
                    _logger.LogTrace(@"Watcher for type ""{type}"" already running.", _plural);
                    return;
                }
            }

            _cancellation = new CancellationTokenSource();

            _watcher = await _client.Watch(_group, _version, _plural,
                TimeSpan.FromSeconds(this.WatcherHttpTimeout),
                OnWatcherEvent,
                OnException,
                OnClose,
                this.Namespace, _cancellation.Token);
            // _metrics.Running.Set(1);
        }

        private void OnWatcherEvent(WatchEventType type, object resource)
        {
            _logger.LogTrace(
                @"Received watch event ""{eventType}"" for ""{plural}"".", type, _plural);

            //  _metrics.WatchedEvents.Inc();

            switch (type)
            {
                case WatchEventType.Added:
                case WatchEventType.Modified:
                case WatchEventType.Deleted:
                    _watchEvents.OnNext((type, resource));
                    break;
                case WatchEventType.Error:
                case WatchEventType.Bookmark:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Event did not match.");
            }
        }

        private async Task RestartWatcher()
        {
            _logger.LogTrace(@"Restarting resource watcher for type ""{type}"".", _plural);
            _cancellation?.Cancel();
            _watcher?.Dispose();
            _watcher = null;
            await WatchResource();
        }

        private void OnException(Exception e)
        {
            _cancellation?.Cancel();
            _watcher?.Dispose();
            _watcher = null;

            // _metrics.Running.Set(0);
            // _metrics.WatcherExceptions.Inc();

            if (e is TaskCanceledException && e.InnerException is IOException)
            {
                _logger.LogTrace(
                    @"Either the server or the client did close the connection on watcher for resource ""{resource}"". Restart.",
                    _plural);
                WatchResource().ConfigureAwait(false);
                return;
            }

            _logger.LogError(e, @"There was an error while watching the resource ""{resource}"".", _plural);
            var backoff = ExponentialBackoff(++_reconnectAttempts);
            _logger.LogInformation("Trying to reconnect with exponential backoff {backoff}.", backoff);
            _resetReconnectCounter?.Dispose();
            _resetReconnectCounter = Observable
                .Timer(TimeSpan.FromMinutes(1))
                .FirstAsync()
                .Subscribe(_ => _reconnectAttempts = 0);

            _reconnectHandler.OnNext(backoff);
        }

        private void OnClose()
        {
            // _metrics.Running.Set(0);
            // _metrics.WatcherClosed.Inc();

            if (_cancellation?.IsCancellationRequested == false)
            {
                _logger.LogDebug("The server closed the connection. Trying to reconnect.");
                var restartTask = RestartWatcher();
                restartTask.Wait();
            }
        }

        private TimeSpan ExponentialBackoff(int retryCount) => TimeSpan
            .FromSeconds(Math.Min(Math.Pow(2, retryCount), MaxRetrySeconds))
            .Add(TimeSpan.FromMilliseconds(_rnd.Next(0, 1000)));
    }
}