using Domain;

namespace Application
{
    public interface IOtpAuthenticator
    {
        Task<string> CreateAndSendOtpAsync(User user);

        Task<User> ValidateOtpAsync(string userPublicId, string otp);
    }
}