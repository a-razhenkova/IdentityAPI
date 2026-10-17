using Domain;

namespace Application
{
    public interface IOtpAuthenticator
    {
        Task<User> AuthenticateAsync(string userReference, string otp, CancellationToken cancellationToken = default);
    }
}