using Domain;

namespace Application
{
    public interface IBearerAuthenticator
    {
        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

        Task<User> AuthenticateByRefreshTokenAsync(Authorization authorization, CancellationToken cancellationToken = default);
    }
}