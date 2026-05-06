using Domain;

namespace Application
{
    public class UserDto
    {
        public string PublicId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public UserRoles Role { get; set; } = UserRoles.Administrator;

        public string Password { get; set; } = string.Empty;

        public UserStatusDto Status { get; set; } = new UserStatusDto();

        public string? Email { get; set; }

        public DateTime RegistrationTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class UserStatusDto
    {
        public UserStatuses Value { get; set; } = UserStatuses.Active;

        public UserStatusReasons Reason { get; set; } = UserStatusReasons.None;

        public string? Note { get; set; }
    }
}