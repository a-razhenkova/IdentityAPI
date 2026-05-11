using Application.RabbitMq;
using Domain;

namespace Application
{
    public static class RabbitMqEventFactory
    {
        public static LoginAttemptMadeEvent CreateLoginAttemptMadeEvent(User user)
            => new LoginAttemptMadeEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Timestamp = user.Password.LastChangedTimestamp,
                IpAddress = user.Login.LastLoginIpAddress
            };

        public static LoginFromNewIpAddressEvent CreateLoginFromNewIpAddressEvent(User user)
            => new LoginFromNewIpAddressEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Timestamp = user.Password.LastChangedTimestamp,
                IpAddress = user.Login.LastLoginIpAddress
            };

        public static EmailVerificationEvent CreateEmailVerificationEvent(string token)
            => new EmailVerificationEvent()
            {
                VerificationToken = token
            };

        public static UserPasswordChangedEvent CreateUserPasswordChangedEvent(User user, string? ipAddress)
            => new UserPasswordChangedEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Timestamp = user.Password.LastChangedTimestamp,
                UserIpAddress = ipAddress
            };
    }
}