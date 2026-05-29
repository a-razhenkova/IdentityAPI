namespace Application
{
    public interface IRabbitMq
    {
        Task PublishEventAsync(object evt, CancellationToken cancellationToken = default);

        Task PublishEventInBackgroundAsync(object evt, CancellationToken cancellationToken = default);
    }
}