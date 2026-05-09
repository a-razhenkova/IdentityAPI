using Domain;

namespace Application
{
    public interface IOtp
    {
        Task<string> CreateAndSendAsync(string username, string password, CancellationToken cancellationToken = default);

        Task<string> CreateAndSaveOtp(User user, CancellationToken cancellationToken = default);

        Task PublishUserOtp(User user, string otp, CancellationToken cancellationToken = default);
    }
}