using Domain;

namespace Application
{
    public interface IClientAuthenticator
    {
        Task<Client> AuthenticateAsync(string key);

        Task<Client> AuthenticateAsync(string key, string secret);
    }
}