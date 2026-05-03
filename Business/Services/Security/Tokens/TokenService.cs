using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Business
{
    public class TokenService : IToken
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IBasicAuthenticator _basicAuthenticator;
        private readonly IBearerAuthenticator _bearerAuthenticator;
        private readonly IOtpAuthenticator _otpAuthenticator;

        public TokenService(IHttpContextAccessor httpContextAccessor,
                           IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                           IBasicAuthenticator basicAuthenticator,
                           IBearerAuthenticator bearerAuthenticator,
                           IOtpAuthenticator otpAuthenticator)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettingsOptions = appSettingsOptions.Value;
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
                // TODO: OpenId
                //case AuthorizationSchema.Bearer:
                //    {
                //        User user = await _bearerAuthenticator.AuthenticateAsync(authorization);
                //        return await CreateAccessTokenAsync(user);
                //    }
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<TokenDto> CreateAccessTokenAsync(Client client)
        {
            var accessToken = new AccessToken(_appSettingsOptions.Security).Create(client);

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
            string accessToken = new AccessToken(_appSettingsOptions.Security).Create(user);
            string refreshToken = new RefreshToken(_appSettingsOptions.Security).Create(user);

            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDto> CreateAccessTokenByOtpAsync(string userExternalId, string otp)
        {
            User user = await _otpAuthenticator.ValidateOtpAsync(userExternalId, otp);
            return await CreateAccessTokenAsync(user);
        }

        public async Task<TokenDto> RefreshAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            User user = await _bearerAuthenticator.AuthenticateByRefreshTokenAsync(authorization);

            string accessToken = new AccessToken(_appSettingsOptions.Security).Create(user);

            return new TokenDto()
            {
                AccessToken = accessToken
            };
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            return await new AccessToken(authorization.Value, _appSettingsOptions.Security).ValidateAsync();
        }
    }
}