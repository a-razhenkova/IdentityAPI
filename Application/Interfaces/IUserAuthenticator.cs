using Domain;

namespace Application
{
    public interface IUserAuthenticator
    {
        Task<User> AuthenticateAsync(string userReference, CancellationToken cancellationToken = default);

        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}