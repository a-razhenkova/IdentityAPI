namespace Application
{
    public interface IRabbitMq
    {
        Task PublishEventAsync(object evt);
    }
}