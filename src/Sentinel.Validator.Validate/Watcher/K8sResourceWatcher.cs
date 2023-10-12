using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sentinel.Core.K8s;
using Sentinel.Validator.Validate.Models;

namespace Sentinel.Validator.Validate.Watcher
{
    public class K8sResourceWatcher
    {
        private readonly ILogger<K8sResourceWatcher> _logger;
        private readonly IKubernetesClient _kubernetesClient;

        public K8sResourceWatcher(ILogger<K8sResourceWatcher> logger, IKubernetesClient kubernetesClient)
        {
            _logger = logger;
            _kubernetesClient = kubernetesClient;
        }

        public Task Watch(ValidationModel validationModel)
        {
            var WatcherHttpTimeout = 86400;

            if (validationModel.K8sResource == null)
            {
                throw new ArgumentNullException(nameof(validationModel.K8sResource));
            }
            else if (validationModel.K8sResource.Plural == null)
            {
                throw new ArgumentNullException(nameof(validationModel.K8sResource.Plural));
            }
            _kubernetesClient.Watch(validationModel.K8sResource.Group,
            validationModel.K8sResource.Version,
            validationModel.K8sResource.Plural, TimeSpan.FromSeconds(WatcherHttpTimeout), (eventType, resource) =>
            {
                _logger.LogInformation($"Event: {eventType}, Resource: {resource}");
                //return Task.CompletedTask;
            });


            return Task.CompletedTask;
            //  throw new NotImplementedException();
        }
    }
}