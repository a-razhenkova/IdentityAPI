namespace Application
{
    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; }

        IUserRepository Users { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(bool hasChanges, CancellationToken cancellationToken = default);
    }
}