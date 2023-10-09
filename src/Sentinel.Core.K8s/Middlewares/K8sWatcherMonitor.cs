using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Core.BackgroundServices;
using Sentinel.Core.K8s.BackgroundServices;

namespace Sentinel.Core.K8s.Middlewares
{
    public static class K8sWatcherMonitor
    {
        // public static List<K8sWatcherAttribute> Attributes{get;set;} = new List<K8sWatcherAttribute>();

        public static Dictionary<string, K8sWatcherMonitorItem> Attributes = new Dictionary<string, K8sWatcherMonitorItem>();
    }

    public class K8sWatcherMonitorItem
    {
       public K8sWatcherAttribute Attribute { get; set; }
        public BackgroundServiceHealthCheck? HealthCheck { get; set; }

        public K8sWatcherMonitorItem(K8sWatcherAttribute attribute)
        {
            Attribute = attribute;
        }

        public K8sWatcherMonitorItem(K8sWatcherAttribute attribute, BackgroundServiceHealthCheck healthCheck)
        {
            Attribute = attribute;
            HealthCheck = healthCheck;
        }
    }
}