using Domain;

namespace Application
{
    public class ClientDto
    {
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public ClientStatusDto Status { get; set; } = new ClientStatusDto();

        public ClientRightDto Right { get; set; } = new ClientRightDto();

        public bool IsInternal { get; set; } = false;

        public IEnumerable<ClientSubscriptionDto> Subscriptions { get; set; } = new List<ClientSubscriptionDto>();
    }

    public class ClientStatusDto
    {
        public ClientStatuses Value { get; set; } = ClientStatuses.Active;

        public ClientStatusReasons Reason { get; set; } = ClientStatusReasons.None;

        public string? Note { get; set; }
    }

    public class ClientRightDto
    {
        public bool CanNotify { get; set; } = false;
    }

    public class ClientSubscriptionDto
    {
        public DateTime ExpirationDate { get; set; } = DateTime.Now;

        public long ContractId { get; set; } = 0;

        public string ContractName { get; set; } = string.Empty;
    }
}