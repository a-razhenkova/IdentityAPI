using Domain;

namespace Application
{
    public interface IOtpAuthenticator
    {
        Task<User> AuthenticateAsync(string userPublicId, string otp, CancellationToken cancellationToken = default);
    }
}