using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sentinel.Core.BackgroundServices
{

    public static partial class BackgroundServiceHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddBackgroundServiceHealthCheck(this IHealthChecksBuilder builder, BackgroundServiceWithHealthCheck bgService)
        {
            return builder.AddTypeActivatedCheck<BackgroundServiceHealthCheck>($"BackgroundServiceHealthCheck {bgService.GetType().ToString()}", null, null, bgService);
        }
    }

    public class BackgroundServiceHealthCheck : IHealthCheck
    {
        public DateTime LastProcessUtc;
        public HealthStatus status;
        public int count = 0;
        public string message = default!;

        public BackgroundServiceHealthCheck ReportHealthy(string message = "")
        {
            LastProcessUtc = DateTime.UtcNow;
            status = HealthStatus.Healthy;
            this.message = message;
            count++;
            return this;
        }

        public BackgroundServiceHealthCheck ReportUnhealthy(string message = "")
        {
            LastProcessUtc = DateTime.UtcNow;
            status = HealthStatus.Unhealthy;
            this.message = message;
            count++;
            return this;
        }

        public BackgroundServiceHealthCheck ReportDegraded(string message = "")
        {
            LastProcessUtc = DateTime.UtcNow;
            status = HealthStatus.Degraded;
            this.message = message;
            count++;
            return this;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            if (LastProcessUtc == DateTime.MinValue || DateTime.UtcNow.AddHours(-1) > LastProcessUtc)
            {
                ReportDegraded("Just started or havent heard over an hour");
            }

            var timeAgo = DateTime.UtcNow.Subtract(LastProcessUtc);
            var data = new Dictionary<string, object> {
                { "Last process", LastProcessUtc },
                { "Time ago", timeAgo },
                {"Count", count.ToString()}
            } as IReadOnlyDictionary<string, object>;

            var result = new HealthCheckResult(status, message, data: data);
            return Task.FromResult(result);
        }
    }
}