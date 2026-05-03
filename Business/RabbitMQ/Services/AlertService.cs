using Business.RabbitMq;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Business
{
    public class AlertService : IAlert
    {
        private readonly IConnection _connection;

        public AlertService(IConnection connection)
        {
            _connection = connection;
        }

        public async Task AddUserPasswordChangedAlertAsync(UserPasswordChangedAlertDto alertDto)
        {
            if (string.IsNullOrWhiteSpace(alertDto.UserEmail))
                return;

            const string routingKey = "user-password-changed";

            using IChannel channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(RabbitMqQueues.UserPasswordChangedAlert, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(RabbitMqQueues.UserPasswordChangedAlert, RabbitMqExchanges.DefaultDirect, routingKey);

            string body = JsonSerializer.Serialize(alertDto);

            await channel.BasicPublishAsync(RabbitMqExchanges.DefaultDirect, routingKey, mandatory: true, body: Encoding.UTF8.GetBytes(body));
        }
    }
}