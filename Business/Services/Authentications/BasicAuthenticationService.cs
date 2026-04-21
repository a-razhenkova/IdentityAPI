using Database.IdentityDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public class BasicAuthenticationService : IBasicAuthenticator
    {
        private readonly IClientAuthenticator _clientAuthenticator;

        public BasicAuthenticationService(IClientAuthenticator clientAuthenticator)
        {
            _clientAuthenticator = clientAuthenticator;
        }

        public async Task<Client> AuthenticateAsync(Authorization authorization)
        {
            if (authorization.Schema != AuthorizationSchema.Basic)
                throw new BadRequestException("Invalid token format.");

            var credentials = Utils.DecodeBasicAuthCredentials(authorization.Value);

            if (!ClientKey.IsValid(credentials.Key) || !ClientSecret.IsValid(credentials.Secret))
                throw new UnauthorizedException("Invalid credentials.");

            return await AuthenticateAsync(credentials.Key, credentials.Secret);
        }

        public async Task<Client> AuthenticateAsync(string key, string secret)
        {
            return await _clientAuthenticator.AuthenticateAsync(key, secret);
        }
    }
}