using Domain;

namespace Application
{
    public interface IBasicAuthenticator
    {
        Task<Client> AuthenticateAsync(Authorization authorization, CancellationToken cancellationToken = default);

        Task<Client> AuthenticateAsync(BasicCredentials credentials, CancellationToken cancellationToken = default);

        Task<Client> AuthenticateAsync(string clientKey, string clientSecret, CancellationToken cancellationToken = default);
    }
}