namespace Application
{
    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; }

        IUserRepository Users { get; }

        Task SaveChangesAsync(bool hasChanges = true);
    }
}