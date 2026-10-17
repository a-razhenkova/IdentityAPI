using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Polly.Registry;
using Shared;

namespace Infrastructure
{
    public sealed class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        public DocumentRepository(IdentityContext context, ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
            : base(context, pipelineProvider)
        {

        }

        public async Task<Document?> GetClientSubscriptionContractAsync(
            string clientKey, long contractId, CancellationToken cancellationToken = default)
        {
            Query = _dbSet
                .Where(d => d.Id == contractId
                         && d.Type == DocumentTypes.SubscriptionContract
                         && d.Subscription.ClientSubscriptions.Any(c => c.Client.Key == clientKey))
                .Include(c => c.Subscription)
                    .ThenInclude(s => s.ClientSubscriptions)
                    .ThenInclude(s => s.Client);

            return await FirstOrDefaultAsync(cancellationToken);
        }
    }
}