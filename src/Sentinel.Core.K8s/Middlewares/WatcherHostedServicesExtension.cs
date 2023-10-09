using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentinel.Core.BackgroundServices;
using Sentinel.Core.K8s.BackgroundServices;
using Sentinel.Core.K8s.Watchers;

namespace Sentinel.Core.K8s.Middlewares
{
    public static class WatcherHostedServicesExtension
    {
        // TODO: Add extension to add watchers to the builder.

        // TODO:
        // Add a Service of ObServableCollection<T> to the builder. as a singleton. 
        // Update the Hosted Service and Add the ObservableCollection to the Hosted service.
        // Add Service.AddHostedService({whatever the type});


        public static void AddK8sWatcherDefinitions(this IServiceCollection services, IConfiguration configuration, params Type[] scanMarkers)
        {

            // var k8sWatcherMonitor =  services.BuildServiceProvider().GetService<K8sWatcherMonitor>();
            // if(k8sWatcherMonitor is null){
            //     services.AddSingleton<K8sWatcherMonitor>();
            //     k8sWatcherMonitor =  services.BuildServiceProvider().GetService<K8sWatcherMonitor>();
            // }

            var k8sWatcherDefinitions = new List<Type>();
            foreach (var marker in scanMarkers)
            {
                var items = marker.Assembly.ExportedTypes.Where(
                    t => t.BaseType != null && t.BaseType.IsGenericType == true
                    && t.BaseType.GetGenericTypeDefinition() == typeof(WatcherBackgroundService<>)
                    && !t.IsInterface && !t.IsAbstract
                    && Attribute.IsDefined(t, typeof(K8sWatcherAttribute))
                );
                if (items != null && items.Any())
                {
                    k8sWatcherDefinitions.AddRange(items);
                }



                // var itemsJtoken = marker.Assembly.ExportedTypes.Where(
                //     t => t.BaseType != null && t.BaseType.IsGenericType == true
                //     && t.BaseType.GetGenericTypeDefinition() == typeof(JTokenWatcherBackgroundService<>)
                //     && !t.IsInterface && !t.IsAbstract
                //     && Attribute.IsDefined(t, typeof(K8sWatcherAttribute))
                // );
                // if (itemsJtoken != null && itemsJtoken.Any())
                // {
                //     k8sWatcherDefinitions.AddRange(itemsJtoken);
                // }
            }
            foreach (var subscribeBackgroundService in k8sWatcherDefinitions)
            {
                K8sWatcherAttribute? watcherAttr = subscribeBackgroundService.GetCustomAttributes(typeof(K8sWatcherAttribute), true).First() as K8sWatcherAttribute;
                if (watcherAttr is not null)
                    K8sWatcherMonitor.Attributes.Add(subscribeBackgroundService.Name, new K8sWatcherMonitorItem(watcherAttr));

                if (watcherAttr != null)
                {
                    bool enabled = watcherAttr.Enabled;
                    if (enabled)
                    {
                        if (subscribeBackgroundService?.BaseType != null)
                        {
                            Type[] genericTypeArguments = subscribeBackgroundService.BaseType.GetGenericArguments();
                            var resourceWatcherType = typeof(ResourceWatcher<>).MakeGenericType(genericTypeArguments);

                            // var resourceWatcherMetricsType = typeof(ResourceWatcherMetrics<>).MakeGenericType(genericTypeArguments);
                            // if (!services.Any(p => p.ServiceType == resourceWatcherType))
                            // {
                            //     services.AddSingleton(resourceWatcherType);
                            // }
                            // if (!services.Any(p => p.ServiceType == resourceWatcherMetricsType))
                            // {
                            //     services.AddSingleton(resourceWatcherMetricsType);
                            // }
                            services.AddHostedServices(subscribeBackgroundService);
                        }
                    }
                }
            }
            //  services.AddSingleton(rabbitMQSubscribeDefinitions as IReadOnlyCollection<SubscribeBackgroundService>);
        }
    }
}
