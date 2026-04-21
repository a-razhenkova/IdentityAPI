using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.Extensions.Options;

namespace Business
{
    public class BearerAuthenticationService : IBearerAuthenticator
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IUserAuthenticator _userAuthenticator;

        public BearerAuthenticationService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                                          IUserAuthenticator userAuthenticator)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _userAuthenticator = userAuthenticator;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            return await _userAuthenticator.AuthenticateAsync(username, password);
        }

        public async Task<User> AuthenticateByRefreshTokenAsync(Authorization authorization)
        {
            if (authorization.Schema != AuthorizationSchema.Bearer)
                throw new BadRequestException("Invalid token format.");

            string userExternalId = new RefreshToken(authorization.Value, _appSettingsOptions.Security)
                .GetClaim(TokenClaim.UserExternalId)
                ?? throw new UnauthorizedException("Invalid token.");

            return await _userAuthenticator.AuthenticateAsync(userExternalId);
        }
    }
}