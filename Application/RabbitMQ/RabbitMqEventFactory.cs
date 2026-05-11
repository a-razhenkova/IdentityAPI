using Application.RabbitMq;
using Domain;

namespace Application
{
    public static class RabbitMqEventFactory
    {
        public static EmailVerificationEvent CreateEmailVerificationEvent(User user, string token)
            => new EmailVerificationEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                VerificationToken = token
            };

        public static NewUserOtpEvent CreateNewUserOtpEvent(User user, string otp)
            => new NewUserOtpEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Otp = otp
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

        public static UserPasswordChangedEvent CreateUserPasswordChangedEvent(User user, string? ipAddress)
            => new UserPasswordChangedEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Timestamp = user.Password.LastChangedTimestamp,
                UserIpAddress = ipAddress
            };

        public static LoginAttemptMadeEvent CreateLoginAttemptMadeEvent(User user)
            => new LoginAttemptMadeEvent()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Username = user.Username,
                Timestamp = user.Password.LastChangedTimestamp,
                IpAddress = user.Login.LastLoginIpAddress
            };
    }
}