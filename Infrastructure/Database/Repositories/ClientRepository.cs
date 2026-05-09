using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(IdentityContext context) : base(context) { }

        public async Task<Client?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
           => await WhereKeyEquals(key, autoTrack: true).SingleOrDefaultAsync(cancellationToken);

        public async Task<Client?> GetByKeyWithNoTrackingAsync(string key, CancellationToken cancellationToken = default)
            => await WhereKeyEquals(key, autoTrack: false).SingleOrDefaultAsync(cancellationToken);

        public async Task<Document?> GetSubscriptionContractWithNoTrackingAsync(string clientKey, long contractId, CancellationToken cancellationToken = default)
        {
            return await _context.ClientSubscription
                .AsNoTracking()
                .Where(c => c.Client.Key.Equals(clientKey)
                         && c.Subscription.Contract.Id == contractId
                         && c.Subscription.Contract.Type == DocumentTypes.SubscriptionContract)
                .Include(c => c.Client)
                .Include(c => c.Subscription).ThenInclude(s => s.Contract)
                .Select(c => c.Subscription.Contract)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
        {
            client.Key = ClientKeyHandler.Create();
            client.Secret = ClientSecretHandler.Create();

            await base.BasicAddAsync(client, cancellationToken);
        }

        public IQueryable<Client> WhereKeyEquals(string key, bool autoTrack = true)
            => Where(x => x.Key.Equals(key), autoTrack: autoTrack);
    }
}