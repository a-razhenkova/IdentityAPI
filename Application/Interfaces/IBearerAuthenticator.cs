using Domain;

namespace Application
{
    public interface IBearerAuthenticator
    {
        Task<User> AuthenticateAsync(string username, string password);

        Task<User> AuthenticateByRefreshTokenAsync(Authorization authorization);
    }
}