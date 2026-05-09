using Domain;

namespace Application
{
    public class BasicAuthenticationService : IBasicAuthenticator
    {
        private readonly IClientAuthenticator _clientAuthenticator;

        public BasicAuthenticationService(IClientAuthenticator clientAuthenticator)
        {
            _clientAuthenticator = clientAuthenticator;
        }

        public async Task<Client> AuthenticateAsync(Authorization authorization, CancellationToken cancellationToken = default)
        {
            var credentials = new BasicCredentials(authorization);
            return await AuthenticateAsync(credentials, cancellationToken);
        }

        public async Task<Client> AuthenticateAsync(BasicCredentials credentials, CancellationToken cancellationToken = default)
        {
            if (!ClientKey.IsValid(credentials.Key) || !ClientSecret.IsValid(credentials.Secret))
                throw new UnauthorizedException("Invalid credentials.");

            return await AuthenticateAsync(credentials.Key, credentials.Secret, cancellationToken);
        }

        public async Task<Client> AuthenticateAsync(string key, string secret, CancellationToken cancellationToken = default)
        {
            return await _clientAuthenticator.AuthenticateAsync(key, secret, cancellationToken);
        }
    }
}