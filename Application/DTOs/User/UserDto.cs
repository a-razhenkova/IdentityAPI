using Domain;

namespace Application
{
    public class UserDto
    {
        public required string Id { get; set; }

        public required string Username { get; set; }

        public required UserRoles Role { get; set; }

        public required UserStatusDto Status { get; set; }

        public string? Email { get; set; }

        public required DateTime RegistrationTimestamp { get; set; }
    }
}