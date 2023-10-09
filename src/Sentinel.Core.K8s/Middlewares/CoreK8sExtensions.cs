using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sentinel.Core.K8s.Middlewares
{
    public static class CoreK8sExtensions
    {
        public static void AddCoreK8s(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["RunOnCluster"] == "true") { services.AddSingleton<KubernetesClientConfiguration>(KubernetesClientConfiguration.InClusterConfig()); }
            else { services.AddSingleton<KubernetesClientConfiguration>(KubernetesClientConfiguration.BuildConfigFromConfigFile()); }

            
            services.AddSingleton<IKubernetesClient, KubernetesClient>();
            services.AddSingleton<KubernetesClient>();
        }
    }
}