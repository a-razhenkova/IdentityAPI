using Application;
using Infrastructure.IdentityDb;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityContext _context;

        public UnitOfWork(IdentityContext context)
        {
            _context = context;

            Clients = new ClientRepository(_context);
            Users = new UserRepository(_context);
        }

        public IClientRepository Clients { get; private set; }

        public IUserRepository Users { get; private set; }

        public async Task SaveChangesAsync(bool hasChanges = true)
        {
            if (hasChanges)
            {
                await _context.SaveChangesAsync();
            }
            else
            {
                return;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}