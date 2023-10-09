using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sentinel.Core.K8s.Middlewares
{
    public class K8sWatcherUIOptions
    {


        public Func<Stream> IndexStream { get; set; } = () => typeof(K8sWatcherUIOptions).GetTypeInfo().Assembly
        .GetManifestResourceStream("Sentinel.Core.K8s.index.html");

        public string RoutePrefix { get; set; } = "k8swatcher";
        public string DocumentTitle { get; set; } = "K8s Watcher UI";
    }
}