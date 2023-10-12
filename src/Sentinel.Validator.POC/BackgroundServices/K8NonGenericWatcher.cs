using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using k8s;
using Sentinel.Core.K8s;
using Sentinel.Core.K8s.Watchers;

namespace Sentinel.Validator.POC.BackgroundServices
{
    public class K8NonGenericWatcher : BackgroundService
    {
        private readonly ILogger<K8NonGenericWatcher> _logger;
        private readonly IKubernetesClient _client;

        public K8NonGenericWatcher(ILogger<K8NonGenericWatcher> logger, IKubernetesClient client)
        {
            _logger = logger;
            _client = client;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("K8NonGenericWatcher is starting.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("K8NonGenericWatcher is working.");

            // var group = "networking.istio.io";
            // var plural = "virtualservices";
            // var version = "v1alpha3";
            // var kubeKind = "VirtualService";

            // var group = "";
            // var plural = "services";
            // var version = "v1";
            // var kubeKind = "Service";

            // var group = "apps";
            // var plural = "deployments";
            // var version = "v1";
            // var kubeKind = "Deployment";

            // var group = "";
            // var plural = "namespaces";
            // var version = "v1";
            // var kubeKind = "Namespace";

            // var group = "networking.istio.io";
            // var plural = "envoyfilters";
            // var version = "v1alpha3";

            var group = "";
            var plural = "pods";
            var version = "v1";

            var number = 1;

            JTokenResourceWatcher watcher = new JTokenResourceWatcher(_client, _logger, group, plural, version);
            await watcher.Start();
            watcher.WatchEvents.Subscribe(
                (x) =>
                {
                    var name = x.Resource.SelectToken("$.metadata.name").ToString();
                    _logger.LogInformation(@"class: {class} ,Event: {Event}, Resource: {name}, order: {number} at {date}", this.GetType().Name, x.Event, name, number++.ToString(), DateTime.Now.ToString());
                }
            );
        }
    }
}