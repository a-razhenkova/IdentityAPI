using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace Application
{
    public class TokenService : IToken
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IBasicAuthenticator _basicAuthenticator;
        private readonly IBearerAuthenticator _bearerAuthenticator;
        private readonly IOtpAuthenticator _otpAuthenticator;

        public TokenService(IHttpContextAccessor httpContextAccessor,
                           IOptionsSnapshot<AppSettings> appSettings,
                           IBasicAuthenticator basicAuthenticator,
                           IBearerAuthenticator bearerAuthenticator,
                           IOtpAuthenticator otpAuthenticator)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _basicAuthenticator = basicAuthenticator;
            _bearerAuthenticator = bearerAuthenticator;
            _otpAuthenticator = otpAuthenticator;
        }

        public async Task<TokenDto> CreateAccessTokenAsync(Authorization authorization)
        {
            switch (authorization.Schema)
            {
                case AuthorizationSchema.Basic:
                    {
                        Client client = await _basicAuthenticator.AuthenticateAsync(authorization);
                        return await CreateAccessTokenAsync(client);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<TokenDto> CreateAccessTokenAsync(Client client)
        {
            var accessToken = new AccessToken(_appSettings.Security).Create(client);

            return new TokenDto()
            {
                AccessToken = accessToken
            };
        }

        public async Task<TokenDto> CreateAccessTokenAsync(string username, string password)
        {
            User user = await _bearerAuthenticator.AuthenticateAsync(username, password);
            return await CreateAccessTokenAsync(user);
        }

        public async Task<TokenDto> CreateAccessTokenAsync(User user)
        {
            string accessToken = new AccessToken(_appSettings.Security).Create(user);
            string refreshToken = new RefreshToken(_appSettings.Security).Create(user);

            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDto> CreateAccessTokenByOtpAsync(string userPublicId, string otp)
        {
            User user = await _otpAuthenticator.ValidateOtpAsync(userPublicId, otp);
            return await CreateAccessTokenAsync(user);
        }

        public async Task<TokenDto> RefreshAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            User user = await _bearerAuthenticator.AuthenticateByRefreshTokenAsync(authorization);
            
            string accessToken = new AccessToken(_appSettings.Security).Create(user);

            return new TokenDto()
            {
                AccessToken = accessToken
            };
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            return await new AccessToken(authorization.Value, _appSettings.Security).ValidateAsync();
        }
    }
}