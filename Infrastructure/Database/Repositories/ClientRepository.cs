using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(IdentityContext context) : base(context)
        {

        }

        public async Task<Client?> GetByKeyAsync(string key, bool autoTrack = false, bool loadStatus = false, bool loadRight = false, bool loadSubscriptions = false)
            => await GetQueryAsync(c => c.Key == key, autoTrack, loadStatus, loadRight, loadSubscriptions).SingleOrDefaultAsync();

        public IQueryable<Client> GetQueryAsync(Expression<Func<Client, bool>> expression,
            bool autoTrack = false,
            bool loadStatus = false,
            bool loadRight = false,
            bool loadSubscriptions = false)
        {
            IQueryable<Client> query = base.GetQueryAsync(expression, autoTrack: autoTrack);

            if (loadStatus)
                query = query.Include(c => c.Status);

            if (loadRight)
                query = query.Include(c => c.Right);

            if (loadSubscriptions)
                query = query.Include(c => c.Subscriptions);

            return query;
        }

        public async Task<Document?> GetSubscriptionContract(string clientKey, long contractId, DocumentTypes documentType)
        {
            return await _context.ClientSubscription
                .Where(c => c.Client.Key.Equals(clientKey)
                         && c.Subscription.Contract.Id == contractId
                         && c.Subscription.Contract.Type == documentType)
                .Include(c => c.Client)
                .Include(c => c.Subscription).ThenInclude(s => s.Contract)
                .AsNoTracking()
                .Select(c => c.Subscription.Contract)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Client client)
        {
            client.Key = ClientKey.Create();
            client.Secret = ClientSecret.Create();

            await _context.AddAsync(client);
        }
    }
}