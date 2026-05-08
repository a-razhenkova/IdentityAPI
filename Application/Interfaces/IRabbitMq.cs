namespace Application
{
    public interface IRabbitMq
    {
        Task PublishEventAsync(object evt, CancellationToken cancellationToken = default);
    }
}