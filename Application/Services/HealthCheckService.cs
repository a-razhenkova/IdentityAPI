using Microsoft.Extensions.Diagnostics.HealthChecks;
using MicrosoftDiagnostics = Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application
{
    public class HealthCheckService : IHealthChecker
    {
        private readonly MicrosoftDiagnostics.HealthCheckService _defaultHealthCheckService;

        public HealthCheckService(MicrosoftDiagnostics.HealthCheckService defaultHealthCheckService)
        {
            _defaultHealthCheckService = defaultHealthCheckService;
        }

        public async Task<HealthReport> CheckHealthAsync(CancellationToken cancellationToken = default)
            => await _defaultHealthCheckService.CheckHealthAsync(cancellationToken);
    }
}