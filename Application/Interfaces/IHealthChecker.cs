using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application
{
    public interface IHealthChecker
    {
        Task<HealthReport> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}