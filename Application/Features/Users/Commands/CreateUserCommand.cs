using Domain;

namespace Application
{
    public sealed record CreateUserCommand
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public required UserRoles Role { get; set; }

        public string? Email { get; set; }
    }
}