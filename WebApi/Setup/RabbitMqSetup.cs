using Application;
using Infrastructure;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;
using Shared;

namespace WebApi
{
    public static class RabbitMqSetup
    {
        public static async Task<WebApplicationBuilder> AddRabbitMqAsync(this WebApplicationBuilder builder)
        {
            try
            {
                string rabbitMqConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.RabbitMq);

                ConnectionSettings settings = ConnectionSettingsBuilder.Create()
                    .Uri(new Uri(rabbitMqConnectionString))
                    .Build();

                IEnvironment environment = AmqpEnvironment.Create(settings);
                IConnection connection = await environment.CreateConnectionAsync();

                builder.Services.AddSingleton(connection);
                builder.Services.AddScoped<IRabbitMq, RabbitMqService>();
            }
            catch
            {
                builder.Services.AddScoped<IRabbitMq, RabbitMqMockService>();
            }

            return builder;
        }
    }
}