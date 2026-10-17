using RabbitMQ.AMQP.Client;

namespace Application
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RabbitMqEventAttribute : Attribute
    {
        public RabbitMqEventAttribute(string queueName)
        {
            QueueName = queueName;
            RoutingKey = queueName;
        }

        #region Exchange

        public string ExchangeName { get; init; } = RabbitMqExchanges.DefaultDirect;
        
        public bool AutoDeleteExchange { get; init; } = false;
        
        public ExchangeType ExchangeType { get; init; } = ExchangeType.DIRECT;

        #endregion

        #region Queue

        public string QueueName { get; init; }

        public bool AutoDeleteQueue { get; init; } = false;

        /// <summary>
        /// Should this queue use be limited to its declaring connection.
        /// </summary>
        /// <remarks>
        /// Such a queue will be deleted when its declaring connection closes.
        /// </remarks>
        public bool IsExclusive { get; init; } = false;

        #endregion

        public string RoutingKey { get; init; }
    }
}