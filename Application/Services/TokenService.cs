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

        public async Task<TokenDto> CreateAccessTokenAsync(Authorization authorization, CancellationToken cancellationToken = default)
        {
            switch (authorization.Schema)
            {
                case AuthorizationSchema.Basic:
                    {
                        Client client = await _basicAuthenticator.AuthenticateAsync(authorization, cancellationToken);
                        return CreateAccessToken(client);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public TokenDto CreateAccessToken(Client client)
        {
            var accessToken = new AccessToken(_appSettings.Security).Create(client);

            return new TokenDto()
            {
                AccessToken = accessToken
            };
        }

        public async Task<TokenDto> CreateAccessTokenAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            User user = await _bearerAuthenticator.AuthenticateAsync(username, password, cancellationToken);
            return CreateAccessToken(user);
        }

        public TokenDto CreateAccessToken(User user)
        {
            string accessToken = new AccessToken(_appSettings.Security).Create(user);
            string refreshToken = new RefreshToken(_appSettings.Security).Create(user);

            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDto> CreateAccessTokenByOtpAsync(string userPublicId, string otp, CancellationToken cancellationToken = default)
        {
            User user = await _otpAuthenticator.AuthenticateAsync(userPublicId, otp, cancellationToken);
            return CreateAccessToken(user);
        }

        public async Task<TokenDto> RefreshAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            User user = await _bearerAuthenticator.AuthenticateByRefreshTokenAsync(authorization, cancellationToken);
            
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