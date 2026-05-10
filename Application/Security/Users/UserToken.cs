using Domain;
using System.Security.Claims;

namespace Application
{
    public abstract class UserToken : ISecurityToken
    {
        protected readonly User _user;

        protected UserToken(User user, SecurityTokenSettings settings)
        {
            _user = user;
            TokenSettings = settings;
        }

        public SecurityTokenSettings TokenSettings { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}