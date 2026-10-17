using Application;
using Infrastructure.IdentityDb;
using Polly.Registry;
using Shared;

namespace Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityContext _context;

        public UnitOfWork(IdentityContext context, ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
        {
            _context = context;

            Clients = new ClientRepository(_context, pipelineProvider);
            Users = new UserRepository(_context, pipelineProvider);
            Documents = new DocumentRepository(_context, pipelineProvider);
        }

        #region Repositories
        public IClientRepository Clients { get; init; }
        public IUserRepository Users { get; init; }
        public IDocumentRepository Documents { get; init; }
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