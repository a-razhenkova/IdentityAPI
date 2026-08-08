using Application;
using Infrastructure.IdentityDb;

namespace Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityContext _context;

        public UnitOfWork(IdentityContext context)
        {
            _context = context;

            Clients = new ClientRepository(_context);
            Users = new UserRepository(_context);
        }

        #region Repositories
        public IClientRepository Clients { get; init; }
        public IUserRepository Users { get; init; }
        #endregion

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(hasChanges: true, cancellationToken);
        }

        public async Task SaveChangesAsync(bool hasChanges, CancellationToken cancellationToken = default)
        {
            if (!hasChanges)
                return;

            await _context.SaveChangesAsync(cancellationToken);
        }

        #region IDispose
        public void Dispose()
        {
            _context.Dispose();
        }
        #endregion
    }
}