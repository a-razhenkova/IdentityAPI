using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace WebApi
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqHealthCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = _serviceProvider.GetRequiredService<IConnection>();

                using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, description: exception.Message);
            }
        }
    }
}