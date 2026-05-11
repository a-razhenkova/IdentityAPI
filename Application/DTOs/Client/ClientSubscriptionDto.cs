namespace Application
{
    public class ClientSubscriptionDto
    {
        public DateTime ExpirationDate { get; set; } = DateTime.Now;

        public long ContractId { get; set; } = 0;

        public string ContractName { get; set; } = string.Empty;
    }
}