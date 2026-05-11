using Application;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using System.Text;
using System.Text.Json;

namespace Infrastructure
{
    public static class RabbitMqExtensions
    {
        public static async Task<PublishResult> PublishEventAsync(this IConnection connection, object evt, RabbitMqEventAttribute settings, CancellationToken cancellationToken = default)
        {
            await connection.EnsureBinding(settings);

            using IPublisher publisher = await connection.PublisherBuilder()
                .Exchange(settings.ExchangeName)
                .Key(settings.RoutingKey)
                .BuildAsync(cancellationToken);

            string message = JsonSerializer.Serialize(evt);
            var amqpMessage = new AmqpMessage(Encoding.UTF8.GetBytes(message));

            return await publisher.PublishAsync(amqpMessage, cancellationToken);
        }

        public static async Task<IQueueSpecification> EnsureQueueDeclaration(this IConnection connection, RabbitMqEventAttribute settings)
        {
            using IManagement management = connection.Management();

            IQueueSpecification queue = management
                .Queue(settings.QueueName)
                .Type(QueueType.CLASSIC)
                .Exclusive(settings.IsExclusive)
                .AutoDelete(settings.AutoDeleteQueue);

            await queue.DeclareAsync();
            return queue;
        }

        public static async Task<IBindingSpecification> EnsureBinding(this IConnection connection, RabbitMqEventAttribute settings)
        {
            using IManagement management = connection.Management();
            IBindingSpecification binding = management.Binding();

            if (settings.ExchangeName == RabbitMqExchanges.DefaultDirect)
            {
                binding.SourceExchange(settings.ExchangeName);
            }
            else
            {
                IExchangeSpecification exchange = management.Exchange(settings.ExchangeName)
                .AutoDelete(settings.AutoDeleteExchange)
                .Type(settings.ExchangeType);

                await exchange.DeclareAsync();
                binding.SourceExchange(exchange);
            }

            IQueueSpecification queue = await connection.EnsureQueueDeclaration(settings);
            binding.DestinationQueue(queue);

            binding.Key(settings.RoutingKey);
            await binding.BindAsync();

            return binding;
        }
    }
}