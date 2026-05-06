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
                new Claim(TokenClaim.IsInternalClient.GetDescription(), _client.IsInternal.ToString(), ClaimValueTypes.Boolean),
                new Claim(TokenClaim.CanNotifyParty.GetDescription(), _client.Right.CanNotifyParty.ToString(), ClaimValueTypes.Boolean),
            ];
    }
}