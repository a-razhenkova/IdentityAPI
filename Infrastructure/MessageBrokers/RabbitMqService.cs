using Application;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using RabbitMQ.AMQP.Client;
using Shared;

namespace Infrastructure
{
    public class RabbitMqService : IRabbitMq
    {
        private readonly ILogger<RabbitMqService> _logger;
        private readonly IConnection _connection;
        private readonly ResiliencePipelineProvider<string> _pipelineProvider;

        public RabbitMqService(ILogger<RabbitMqService> logger,
                              IConnection connection,
                              ResiliencePipelineProvider<string> pipelineProvider)
        {
            _logger = logger;
            _connection = connection;
            _pipelineProvider = pipelineProvider;
        }

        public async Task PublishEventAsync(object evt, CancellationToken cancellationToken = default)
        {
            var settings = evt.GetRequiredCustomAttribute<RabbitMqEventAttribute>();

            ResiliencePipeline pipeline = _pipelineProvider.GetPipeline(ResiliencePipelines.RabbitMQ_Publish);
            await pipeline.ExecuteAsync(async (cancellationToken) => await PublishEventAsync(evt, settings, cancellationToken), cancellationToken);
        }

        private async Task PublishEventAsync(object evt, RabbitMqEventAttribute settings, CancellationToken cancellationToken = default)
        {
            try
            {
                PublishResult result = await _connection.PublishEventAsync(evt, settings, cancellationToken);

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
                throw;
            }
        }
    }
}