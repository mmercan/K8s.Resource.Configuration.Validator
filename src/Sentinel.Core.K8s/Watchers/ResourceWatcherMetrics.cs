using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using Prometheus;
using Sentinel.Core.K8s.Extensions;
using static Sentinel.Core.K8s.Watchers.ResourceWatcherMetrics;

namespace Sentinel.Core.K8s.Watchers
{
    public static class ResourceWatcherMetrics
    {
        public static readonly string[] Labels = { "operator", "kind", "group", "version", "scope", };
    }

    public class ResourceWatcherMetrics<TEntity>
     where TEntity : IKubernetesObject<V1ObjectMeta>
    {

#pragma warning disable CS8620
        public ResourceWatcherMetrics(IOptions<OperatorSettings<TEntity>> settings)
        {
            var crd = CustomEntityDefinitionExtensions.CreateResourceDefinition<TEntity>();
            var labelValues = new[]
            {
                settings.Value?.Name,
                crd.KubernetesEntity.Kind,
                crd.KubernetesEntity.Group,
                crd.KubernetesEntity.ApiVersion,
                crd.Scope.ToString(),
            };

            Running = Metrics
                .CreateGauge(
                    "operator_resource_watcher_running",
                    "Determines if the resource watcher is up and running (1 == Running, 0 == Stopped)",
                    Labels)
                .WithLabels(labelValues);

            WatchedEvents = Metrics
                .CreateCounter(
                    "operator_resource_watcher_events",
                    "The count of totally watched events from the resource watcher",
                    Labels)
                .WithLabels(labelValues);

            WatcherExceptions = Metrics
                .CreateCounter(
                    "operator_resource_watcher_exceptions",
                    "The count of observed thrown exceptions from the resource watcher",
                    Labels)
                .WithLabels(labelValues);

            WatcherClosed = Metrics
                .CreateCounter(
                    "operator_resource_watcher_closed",
                    "The count of observed 'close' events of the resource watcher",
                    Labels)
                .WithLabels(labelValues);
        }

        public Gauge.Child Running { get; }

        public Counter.Child WatchedEvents { get; }

        public Counter.Child WatcherExceptions { get; }

        public Counter.Child WatcherClosed { get; }
    }
}