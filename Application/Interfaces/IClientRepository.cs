using Domain;
using Shared;
using System.Linq.Expressions;

namespace Application
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetByKeyAsync(string key, bool autoTrack = false, bool loadStatus = false, bool loadRight = false, bool loadSubscriptions = false);

        IQueryable<Client> GetQueryAsync(Expression<Func<Client, bool>> expression,
            bool autoTrack = false,
            bool loadStatus = false,
            bool loadRight = false,
            bool loadSubscriptions = false);

        Task<Document?> GetSubscriptionContract(string clientKey, long contractId, DocumentTypes documentType);

        Task AddAsync(Client client);
    }
}