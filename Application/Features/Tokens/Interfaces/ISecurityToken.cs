using System.Security.Claims;

namespace Application
{
    public interface ISecurityToken
    {
        SecurityTokenSettings TokenSettings { get; init; }

        List<Claim> CreateClaims();
    }
}