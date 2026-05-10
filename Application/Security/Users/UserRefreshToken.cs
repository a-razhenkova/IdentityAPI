using Domain;
using Shared;
using System.Security.Claims;

namespace Application
{
    public class UserRefreshToken : UserToken
    {
        public UserRefreshToken(User user, SecurityTokenSettings settings)
            : base(user, settings)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.UserPublicId.GetDescription(), _user.PublicId)
            ];
    }
}