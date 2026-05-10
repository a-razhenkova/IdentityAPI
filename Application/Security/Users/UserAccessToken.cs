using Domain;
using Shared;
using System.Security.Claims;

namespace Application
{
    public class UserAccessToken : UserToken
    {
        public UserAccessToken(User user, SecurityTokenSettings settings)
            : base(user, settings)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.UserPublicId.GetDescription(), _user.PublicId),
                new Claim(TokenClaim.Username.GetDescription(), _user.Username),
                new Claim(TokenClaim.UserRole.GetDescription(), _user.Role.ToString()),
                new Claim(TokenClaim.UserStatus.GetDescription(), _user.Status.Value.ToString())
            ];
    }
}