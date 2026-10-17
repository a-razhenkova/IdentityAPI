namespace Application
{
    public class ClientDto
    {
        public required string Key { get; set; }

        public required string Name { get; set; }

        public required ClientStatusDto Status { get; set; }

        public required ClientRightDto Right { get; set; }

        public required bool IsInternal { get; set; }

        public ICollection<ClientSubscriptionDto>? Subscriptions { get; set; }
    }
}