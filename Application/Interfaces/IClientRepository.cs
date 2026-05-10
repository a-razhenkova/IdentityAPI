using Domain;

namespace Application
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
        Task<Client?> GetByKeyWithNoTrackingAsync(string key, CancellationToken cancellationToken = default);

        Task<Document?> GetSubscriptionContractWithNoTrackingAsync(string clientKey, long contractId, CancellationToken cancellationToken = default);

        IQueryable<Client> WhereKeyEquals(string key, bool autoTrack = true);
    }
}