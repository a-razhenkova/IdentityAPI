using Domain;
using Shared;
using System.Security.Claims;

namespace Application
{
    public class ClientAccessToken : ClientToken
    {
        public ClientAccessToken(Client client, SecurityTokenSettings settings)
            : base(client, settings)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.ClientId.GetDescription(), _client.Key),
                new Claim(TokenClaim.ClientStatus.GetDescription(), _client.Status.Value.ToString().ToUpper()),
                new Claim(TokenClaim.IsInternalClient.GetDescription(), _client.IsInternal.ToString(), ClaimValueTypes.Boolean),
                new Claim(TokenClaim.CanNotify.GetDescription(), _client.Right.CanNotify.ToString(), ClaimValueTypes.Boolean),
            ];
    }
}