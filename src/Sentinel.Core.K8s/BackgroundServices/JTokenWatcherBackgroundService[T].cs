
// using k8s;
// using k8s.Models;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Diagnostics.HealthChecks;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Newtonsoft.Json.Linq;
// using Sentinel.Core.BackgroundServices;
// using Sentinel.Core.K8s.Middlewares;
// using Sentinel.Core.K8s.Watchers;

// namespace Sentinel.Core.K8s.BackgroundServices
// {
//     public abstract class JTokenWatcherBackgroundService<TEntity> : BackgroundServiceWithHealthCheck
//     where TEntity : IKubernetesObject<V1ObjectMeta>
//     {
//         protected readonly IConfiguration _configuration;
//         private readonly IKubernetesClient _client;
//         protected string Name { get; set; } = default!;
//         protected string? Namespace { get; set; } = default!;
//         public JTokenWatcher<TEntity> Watcher { get; set; } = default!;
//         private string AppName
//         {
//             get { return this.GetType().Name; }
//         }
//         public JTokenWatcherBackgroundService(IConfiguration configuration, IKubernetesClient client,
//             ILogger<WatcherBackgroundService<TEntity>> logger, IOptions<HealthCheckServiceOptions> hcoptions) : base(logger, hcoptions)
//         {
//             _configuration = configuration;
//             _client = client;

//             getAttributeDetails();

//             var settings = Options.Create(new OperatorSettings<TEntity>());
//             settings.Value.Namespace = Namespace;
//             settings.Value.Name = Name;
//             settings.Value.WatcherHttpTimeout = 86400;
//             ResourceWatcherMetrics<TEntity> metrics = new ResourceWatcherMetrics<TEntity>(settings);

//             Watcher = new JTokenWatcher<TEntity>(client, logger, metrics, settings);
//             executingTask = Task.CompletedTask;
//         }

//         public abstract void Watch(WatchEventType Event, object Resource);


//         protected override Task Execute(CancellationToken stoppingToken)
//         {
//             Watcher.Start();
//             Watcher.WatchEvents.Subscribe(x =>
//                         {

//                             var resourceName = x.Resource.SelectTokens("metadata.name").ToString();
//                             var healthstat = ReportHealthy(string.Format("{0} watcher event: {1} :  {2}", appName, x.Event, resourceName));
//                             var name = this.GetType().Name;
//                             var monitorItem = K8sWatcherMonitor.Attributes[name];
//                             if (monitorItem is not null)
//                             {
//                                 monitorItem.HealthCheck = healthstat;
//                             }
//                             Watch(x.Event, x.Resource);
//                         });
//             return Task.CompletedTask;
//         }


//         private void getAttributeDetails()
//         {
//             Type t = this.GetType();
//             // Get instance of the attribute.

//             var attribute = Attribute.GetCustomAttribute(t, typeof(K8sWatcherAttribute));
//             K8sWatcherAttribute? watcherAttribute = attribute as K8sWatcherAttribute;
//             if (watcherAttribute == null)
//             {
//                 _logger.LogWarning("K8sWatcherAttribute The attribute was not found. on {appName}", appName);
//             }
//             else
//             {
//                 var AppName = t.Name;
//                 Name = string.IsNullOrEmpty(watcherAttribute.Name) ? t.Name : watcherAttribute.Name;
//                 if (watcherAttribute.TimeoutTotalMinutes > 0)
//                 {
//                     timeout = TimeSpan.FromMinutes(watcherAttribute.TimeoutTotalMinutes);
//                 }
//                 _logger.LogInformation("The Name Attribute is: {0}. Namespace : {Namespace}, Watch All Namespaces {WatchAll} Enabled : {Enabled}",
//                  watcherAttribute.Name, watcherAttribute.Namespace, watcherAttribute.WatchAllNamespaces, watcherAttribute.Enabled);
//                 _logger.LogInformation("The Description Attribute is: {0}.", watcherAttribute.Description);
//                 if (watcherAttribute.WatchAllNamespaces)
//                 {
//                     Namespace = null;
//                 }
//                 else
//                 {
//                     Namespace = watcherAttribute.Namespace;
//                 }

//             }
//         }
//     }
// }
