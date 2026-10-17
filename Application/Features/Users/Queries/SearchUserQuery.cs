using Domain;

namespace Application
{
    public sealed record SearchUserQuery : PaginatedQuery
    {
        public string? Username { get; set; }

        public UserRoles? Role { get; set; }

        public UserStatuses? Status { get; set; }
    }
}