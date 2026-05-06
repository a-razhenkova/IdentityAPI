using Shared;

namespace Application
{
    public class ClientSearchParams : PaginationParams
    {
        public string? Key { get; set; }

        public string? Name { get; set; }

        public ClientStatuses? Status { get; set; }

        public bool? CanNotifyParty { get; set; }
    }
}