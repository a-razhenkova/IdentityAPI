namespace Application
{
    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; init; }
        IUserRepository Users { get; init; }

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(bool hasChanges, CancellationToken cancellationToken = default);
    }
}