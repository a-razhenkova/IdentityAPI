using Domain;

namespace Application
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public UserRoles Role { get; set; } = UserRoles.Administrator;

        public UserStatusDto Status { get; set; } = new UserStatusDto();

        public string? Email { get; set; }

        public DateTime RegistrationTimestamp { get; set; } = DateTime.UtcNow;
    }
}