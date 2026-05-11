namespace Application
{
    public class ClientDto
    {
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public ClientStatusDto Status { get; set; } = new ClientStatusDto();

        public ClientRightDto Right { get; set; } = new ClientRightDto();

        public bool IsInternal { get; set; } = false;

        public ICollection<ClientSubscriptionDto> Subscriptions { get; set; } = new List<ClientSubscriptionDto>();
    }
}