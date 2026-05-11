using Domain;

namespace Application
{
    public class ClientStatusDto
    {
        public ClientStatuses Value { get; set; } = ClientStatuses.Active;

        public ClientStatusReasons Reason { get; set; } = ClientStatusReasons.None;

        public string? Note { get; set; }
    }
}