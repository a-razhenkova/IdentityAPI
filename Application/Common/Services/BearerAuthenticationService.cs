using Domain;
using Microsoft.Extensions.Options;
using Shared;

namespace Application
{
    public class BearerAuthenticationService : IBearerAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUserAuthenticator _userAuthenticator;

        public BearerAuthenticationService(
            IOptionsSnapshot<AppSettings> appSettings,
            IUserAuthenticator userAuthenticator)
        {
            _appSettings = appSettings.Value;
            _userAuthenticator = userAuthenticator;
        }

        public async Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            return await _userAuthenticator.AuthenticateAsync(username, password, cancellationToken);
        }

        public async Task<User> AuthenticateByRefreshTokenAsync(Authorization authorization, CancellationToken cancellationToken = default)
        {
            if (authorization.Schema != AuthorizationSchema.Bearer)
                throw new BadRequestException(AppMessages.InvalidTokenFormat);

            string userReference = new RefreshToken(authorization.Value, _appSettings.Security)
                .GetClaim(TokenClaim.UserReference) ?? throw new UnauthorizedException(AppMessages.InvalidToken);

            return await _userAuthenticator.AuthenticateAsync(userReference, cancellationToken);
        }
    }
}