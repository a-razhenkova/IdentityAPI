using Domain;

namespace Application
{
    public interface IUserAuthenticator
    {
        Task<User> AuthenticateAsync(string userPublicId);

        Task<User> AuthenticateAsync(string username, string password);
    }
}