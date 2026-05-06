using Domain;

namespace Application
{
    public interface IBasicAuthenticator
    {
        Task<Client> AuthenticateAsync(BasicCredentials credentials);

        Task<Client> AuthenticateAsync(string clientKey, string clientSecret);
    }
}