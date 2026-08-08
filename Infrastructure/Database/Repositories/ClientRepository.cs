using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public sealed class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(IdentityContext context) : base(context) { }

        public async Task<Client?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
           return await WhereKeyEquals(key, autoTrack: true)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<Client?> GetByKeyWithNoTrackingAsync(string key, CancellationToken cancellationToken = default)
        {
            return await WhereKeyEquals(key, autoTrack: false)
                .SingleOrDefaultAsync(cancellationToken);
        }
        
        public async Task<Document?> GetSubscriptionContractWithNoTrackingAsync(string clientKey, long contractId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Key.Equals(clientKey)
                         && c.Subscriptions.Any(s => s.Subscription.ContractId == contractId
                                                  && s.Subscription.Contract.Type == DocumentTypes.SubscriptionContract))
                .Include(c => c.Subscriptions)
                    .ThenInclude(s => s.Subscription)
                    .ThenInclude(s => s.Contract)
                .SelectMany(c => c.Subscriptions.Select(s => s.Subscription.Contract))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<Client> WhereKeyEquals(string key, bool autoTrack = true)
        {
            return Where(c => c.Key.Equals(key), autoTrack: autoTrack);
        }

        public override IQueryable<Client> Where(Expression<Func<Client, bool>> expression, bool autoTrack = true)
        {
            return base.Where(expression, autoTrack)
                .Include(c => c.Status)
                .Include(c => c.Right);
        }
    }
}