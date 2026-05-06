using Domain;
using Microsoft.Extensions.Options;
using Shared;

namespace Application
{
    public class BearerAuthenticationService : IBearerAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUserAuthenticator _userAuthenticator;

        public BearerAuthenticationService(IOptionsSnapshot<AppSettings> appSettings,
                                          IUserAuthenticator userAuthenticator)
        {
            _appSettings = appSettings.Value;
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

            string userPublicId = new RefreshToken(authorization.Value, _appSettings.Security)
                .GetClaim(TokenClaim.UserPublicId)
                ?? throw new UnauthorizedException("Invalid token.");

            return await _userAuthenticator.AuthenticateAsync(userPublicId);
        }
    }
}