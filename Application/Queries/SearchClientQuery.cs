using Domain;

namespace Application
{
    public sealed record SearchClientQuery : PaginatedQuery
    {
        public string? Key { get; set; }

        public string? Name { get; set; }

        public ClientStatuses? Status { get; set; }

        public bool? CanNotify { get; set; }
    }
}