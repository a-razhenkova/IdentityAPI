using Application.RabbitMq;

namespace Application
{
    public interface IRabbitMq
    {
        Task PublishUserPasswordChangedEventAsync (UserPasswordChangedEvent message);
    }
}