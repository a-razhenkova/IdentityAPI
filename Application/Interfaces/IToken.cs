using Domain;
using Microsoft.IdentityModel.Tokens;

namespace Application
{
    public interface IToken
    {
        Task<TokenDto> CreateAccessTokenAsync(Authorization authorization, CancellationToken cancellationToken = default);
        TokenDto CreateAccessToken(Client client);
        
        Task<TokenDto> CreateAccessTokenAsync(string username, string password, CancellationToken cancellationToken = default);
        TokenDto CreateAccessToken(User user);

        Task<TokenDto> CreateAccessTokenByOtpAsync(string userPublicId, string otp, CancellationToken cancellationToken = default);

        Task<TokenDto> RefreshAccessTokenAsync(CancellationToken cancellationToken = default);

        Task<TokenValidationResult> ValidateAccessTokenAsync();
    }
}