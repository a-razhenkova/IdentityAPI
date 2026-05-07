using Domain;

namespace Application
{
    public interface IBasicAuthenticator
    {
        Task<Client> AuthenticateAsync(Authorization authorization);

        Task<Client> AuthenticateAsync(BasicCredentials credentials);

        Task<Client> AuthenticateAsync(string clientKey, string clientSecret);
    }
}