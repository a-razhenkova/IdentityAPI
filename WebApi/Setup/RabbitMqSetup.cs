using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using Shared;

namespace WebApi
{
    public static class RabbitMqSetup
    {
        public static async Task<WebApplicationBuilder> AddRabbitMqAsync(this WebApplicationBuilder builder)
        {
            string rabbitMqConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.RabbitMq);

            ConnectionSettings settings = ConnectionSettingsBuilder.Create()
                .Uri(new Uri(rabbitMqConnectionString))
                .Build();

            IEnvironment environment = AmqpEnvironment.Create(settings);
            IConnection connection = await environment.CreateConnectionAsync();

            builder.Services.AddSingleton(connection);

            return builder;
        }
    }
}