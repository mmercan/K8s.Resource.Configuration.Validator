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
using Newtonsoft.Json.Linq;
using Sentinel.Core.K8s.Extensions;
using Sentinel.Core.K8s.Models.Entities;

namespace Sentinel.Core.K8s.Watchers
{

#nullable enable
    public sealed class JTokenWatcher<TEntity> : IDisposable
           where TEntity : IKubernetesObject<V1ObjectMeta>
    {
        private const double MaxRetrySeconds = 32;

        private readonly Subject<(WatchEventType Event, object Resource)> _watchEvents = new();
        private readonly IKubernetesClient _client;
        private readonly ILogger _logger;
        private readonly ResourceWatcherMetrics<TEntity> _metrics;
        private readonly IOptions<OperatorSettings<TEntity>> _settings;
        private readonly Subject<TimeSpan> _reconnectHandler = new();
        private readonly IDisposable _reconnectSubscription;
        private readonly Random _rnd = new();

        private IDisposable? _resetReconnectCounter;
        private int _reconnectAttempts;
        private CancellationTokenSource? _cancellation;
        private Watcher<object>? _watcher;
        private string? Namespace;
        private uint WatcherHttpTimeout;

        private KubernetesEntityAttribute kubernetesEntity { get; set; }
        private EntityScope entityScope { get; set; }

        public JTokenWatcher(
            IKubernetesClient client,
            ILogger logger,
            ResourceWatcherMetrics<TEntity> metrics,
            IOptions<OperatorSettings<TEntity>> settings)
        {
            _client = client;
            _logger = logger;
            _metrics = metrics;
            _settings = settings;

            var definition = CustomEntityDefinitionExtensions.CreateResourceDefinition<TEntity>();
            kubernetesEntity = definition.KubernetesEntity;
            entityScope = definition.Scope;

            if (settings.Value != null)
            {
                this.Namespace = settings.Value.Namespace;
                this.WatcherHttpTimeout = settings.Value.WatcherHttpTimeout;

            }
            else
            {
                this.Namespace = null;
                this.WatcherHttpTimeout = 86400; //24 hours
            }

            _reconnectSubscription =
                _reconnectHandler
                    .Select(Observable.Timer)
                    .Switch()
                    .Subscribe(async _ => await WatchResource());
        }

        public IObservable<(WatchEventType Event, object Resource)> WatchEvents => _watchEvents;

        public Task Start()
        {
            _logger.LogDebug(@"Resource Watcher startup for type ""{type}"".", typeof(TEntity));
            return WatchResource();
        }

        public Task Stop()
        {
            _logger.LogTrace(@"Resource Watcher shutdown for type ""{type}"".", typeof(TEntity));
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
            _logger.LogTrace(@"Disposed resource watcher for type ""{type}"".", typeof(TEntity));
            _metrics.Running.Set(0);
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
                    _logger.LogTrace(@"Watcher for type ""{type}"" already running.", typeof(TEntity));
                    return;
                }
            }

            _cancellation = new CancellationTokenSource();

            _watcher = await _client.Watch(kubernetesEntity.Group, kubernetesEntity.ApiVersion, kubernetesEntity.PluralName,
                TimeSpan.FromSeconds(this.WatcherHttpTimeout),
                OnWatcherEvent,
                OnException,
                OnClose,
                this.Namespace,
                _cancellation.Token);
            _metrics.Running.Set(1);
        }

        private void OnWatcherEvent(WatchEventType type, object resource)
        {
            _logger.LogTrace(
                @"Received watch event ""{eventType}"" for ""{kind}/{name}"".",
                type,
                nameof(TEntity),
                resource.ToString());

            _metrics.WatchedEvents.Inc();

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
            _logger.LogTrace(@"Restarting resource watcher for type ""{type}"".", typeof(TEntity));
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

            _metrics.Running.Set(0);
            _metrics.WatcherExceptions.Inc();

            if (e is TaskCanceledException && e.InnerException is IOException)
            {
                _logger.LogTrace(
                    @"Either the server or the client did close the connection on watcher for resource ""{resource}"". Restart.",
                    typeof(TEntity));
                WatchResource().ConfigureAwait(false);
                return;
            }

            _logger.LogError(e, @"There was an error while watching the resource ""{resource}"".", typeof(TEntity));
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
            _metrics.Running.Set(0);
            _metrics.WatcherClosed.Inc();

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