using Application;
using Domain;
using Polly.Registry;
using Shared;

namespace Infrastructure
{
    public sealed class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(IdentityContext context, ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
            : base(context, pipelineProvider)
        {

        }
    }
}