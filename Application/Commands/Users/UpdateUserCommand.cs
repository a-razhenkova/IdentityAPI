using Domain;

namespace Application
{
    public sealed record UpdateUserCommand
    {
        public required string Username { get; set; }

        public required UserRoles Role { get; set; }

        public required UpdateUserStatusCommand Status { get; set; }
    }
}