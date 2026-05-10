using Domain;
using System.Security.Claims;

namespace Application
{
    public abstract class ClientToken : ISecurityToken
    {
        protected readonly Client _client;

        protected ClientToken(Client client, SecurityTokenSettings settings)
        {
            _client = client;
            TokenSettings = settings;
        }

        public SecurityTokenSettings TokenSettings { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}