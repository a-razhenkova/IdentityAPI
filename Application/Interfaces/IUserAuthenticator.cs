using Domain;

namespace Application
{
    public interface IUserAuthenticator
    {
        Task<User> AuthenticateAsync(string publicId, CancellationToken cancellationToken = default);

        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}