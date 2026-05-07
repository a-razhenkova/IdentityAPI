using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.AMQP.Client;

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
                
                if (connection.State != State.Open)
                    throw new ArgumentException($"State: {connection.State}");

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, description: exception.Message);
            }
        }
    }
}