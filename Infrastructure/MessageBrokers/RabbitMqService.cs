using Application;
using Application.RabbitMq;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure
{
    public class RabbitMqService : IRabbitMq
    {
        private readonly IConnection _connection;

        public RabbitMqService(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishUserPasswordChangedEventAsync(UserPasswordChangedEvent message)
        {
            if (string.IsNullOrWhiteSpace(message.UserEmail))
                return;

            const string routingKey = "user-password-changed";

            using IChannel channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(RabbitMqQueues.UserPasswordChangedEvent, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(RabbitMqQueues.UserPasswordChangedEvent, RabbitMqExchanges.DefaultDirect, routingKey);

            string body = JsonSerializer.Serialize(message);
            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(RabbitMqExchanges.DefaultDirect, routingKey, mandatory: true, basicProperties: properties, body: Encoding.UTF8.GetBytes(body));
        }
    }
}