using Domain;

namespace Application
{
    public interface IClientAuthenticator
    {
        Task<Client> AuthenticateAsync(string key, CancellationToken cancellationToken = default);

        Task<Client> AuthenticateAsync(string key, string secret, CancellationToken cancellationToken = default);
    }
}