using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MetricsDemoApi.HealthChecks;

public class DemoHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("Demo API is healthy"));
        }

        return Task.FromResult(
            HealthCheckResult.Unhealthy("Demo API is unhealthy"));
    }
}