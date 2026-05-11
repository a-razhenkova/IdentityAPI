using Domain;

namespace WebApi.V1
{
    public class SearchClientRequest : SearchRequest
    {
        public string? Key { get; set; }

        public string? Name { get; set; }

        public ClientStatuses? Status { get; set; }

        public bool? CanNotify { get; set; }
    }
}