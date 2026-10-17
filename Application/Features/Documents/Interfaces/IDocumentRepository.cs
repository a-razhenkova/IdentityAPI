using Domain;

namespace Application
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<Document?> GetClientSubscriptionContractAsync(string clientKey, long contractId, CancellationToken cancellationToken = default);
    }
}