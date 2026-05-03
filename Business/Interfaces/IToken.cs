using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Business
{
    public interface IToken
    {
        Task<TokenDto> CreateAccessTokenAsync(Authorization authorization);
        Task<TokenDto> CreateAccessTokenAsync(Client client);
        
        Task<TokenDto> CreateAccessTokenAsync(string username, string password);
        Task<TokenDto> CreateAccessTokenAsync(User user);

        Task<TokenDto> CreateAccessTokenByOtpAsync(string userExternalId, string otp);

        Task<TokenDto> RefreshAccessTokenAsync();

        Task<TokenValidationResult> ValidateAccessTokenAsync();
    }
}