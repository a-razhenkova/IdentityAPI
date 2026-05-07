using Application;
using Microsoft.Extensions.Logging;
using RabbitMQ.AMQP.Client;
using Shared;

namespace Infrastructure
{
    public class RabbitMqService : IRabbitMq
    {
        private readonly ILogger<RabbitMqService> _logger;
        private readonly IConnection _connection;

        public RabbitMqService(ILogger<RabbitMqService> logger, IConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public async Task PublishEventAsync(object evt)
        {
            try
            {
                var settings = evt.GetRequiredCustomAttribute<RabbitMqEventAttribute>();

                PublishResult result = await _connection.PublishEventAsync(evt, settings);

                if (result.Outcome.State == OutcomeState.Accepted)
                {
                    _logger.LogInformation($"An event was sent to queue '{settings.QueueName}' with key '{settings.RoutingKey}'.");
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected publish outcome: {result.Outcome.State}\nError: {result.Outcome.Error?.ToString()}");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }
}