using Domain;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class ClientModel
    {
        /// <summary>
        /// Client key.
        /// </summary>
        /// <example>dba1d25a-0062-49e7-b4f0-31224a69f9e4</example>
        [StringLength(Constants.UidLength)]
        public required string Key { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        /// <example>Auth API</example>
        [StringLength(ClientConstants.Name)]
        [RegularExpression(ClientConstants.NameRegex)]
        public required string Name { get; set; }

        /// <summary>
        /// Client status.
        /// </summary>
        public required ClientStatusModel Status { get; set; }

        /// <summary>
        /// Client right.
        /// </summary>
        public required ClientRightModel Right { get; set; }

        public required IEnumerable<ClientSubscriptionModel> Subscriptions { get; set; }
    }
}