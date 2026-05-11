namespace WebApi.V1
{
    public class ClientSubscriptionResponse
    {
        /// <summary>
        /// The expiration date of the subscription.
        /// </summary>
        public required DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The contract identifier associated with the subscription.
        /// </summary>
        public required long ContractId { get; set; }

        /// <summary>
        /// The name of the contract associated with the subscription.
        /// </summary>
        /// <example>test.pdf</example>
        public required string ContractName { get; set; }
    }
}