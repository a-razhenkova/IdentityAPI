using Application;
using Domain;

namespace Tests.Mocks
{
    public class UserMock
    {
        public static User CreateBasicUser(
            string? publicId = default,
            string? username = default,
            UserRoles role = UserRoles.Administrator,
            string? otpSecret = default,
            string? email = "test@test.bg",
            bool isVerified = true,
            DateTime? RegistrationTimestamp = default,
            UserStatuses status = UserStatuses.Active,
            UserStatusReasons statusReason = UserStatusReasons.None)
        {
            return new User()
            {
                Id = new Random().Next(100, 1000),
                PublicId = string.IsNullOrWhiteSpace(publicId) ? Guid.NewGuid().ToString() : publicId,
                Username = string.IsNullOrWhiteSpace(username) ? Guid.NewGuid().ToString() : username,
                Role = role,
                OtpSecret = string.IsNullOrWhiteSpace(otpSecret) ? UserOtpSecret.Create() : otpSecret,
                Email = email,
                IsVerified = isVerified,
                RegistrationTimestamp = RegistrationTimestamp ?? DateTime.Now,
                Status = new UserStatus()
                {
                    Id = new Random().Next(100, 1000),
                    Value = status,
                    Reason = statusReason
                }
            };
        }
    }
}