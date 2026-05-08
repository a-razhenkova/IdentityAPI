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
        private readonly ResiliencePipelineProvider<ResiliencePipelineType> _pipelineProvider;

        public RabbitMqService(ILogger<RabbitMqService> logger,
                              IConnection connection,
                              ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
        {
            _logger = logger;
            _connection = connection;
            _pipelineProvider = pipelineProvider;
        }

        public async Task PublishEventAsync(object evt, CancellationToken cancellationToken = default)
        {
            var settings = evt.GetRequiredCustomAttribute<RabbitMqEventAttribute>();

            ResiliencePipeline pipeline = _pipelineProvider.GetPipeline(ResiliencePipelineType.RabbitMQ_PublishFastEvent);
            await pipeline.ExecuteAsync(async (cancellationToken) => await PublishEventAsync(evt, settings, cancellationToken), cancellationToken);
        }

        public async Task PublishEventInBackground(object evt, CancellationToken cancellationToken = default)
        {
            var settings = evt.GetRequiredCustomAttribute<RabbitMqEventAttribute>();

            ResiliencePipeline pipeline = _pipelineProvider.GetPipeline(ResiliencePipelineType.RabbitMQ_PublishEventInBackground);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            pipeline.ExecuteAsync(async (cancellationToken) => await PublishEventAsync(evt, settings, cancellationToken), cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task PublishEventAsync(object evt, RabbitMqEventAttribute settings, CancellationToken cancellationToken = default)
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
    }
}