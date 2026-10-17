using Application;
using Domain;
using Polly.Registry;
using Shared;

namespace Infrastructure.IdentityDb
{
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IdentityContext context, ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
            : base(context, pipelineProvider)
        {

        }
    }
}