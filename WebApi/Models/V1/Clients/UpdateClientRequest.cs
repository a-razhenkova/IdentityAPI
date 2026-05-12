using Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UpdateClientRequest
    {
        /// <summary>
        /// Name.
        /// </summary>
        /// <example>Auth API</example>
        [StringLength(ClientConstants.NameMaxLength)]
        [RegularExpression(ClientConstants.NameRegex)]
        public required string Name { get; set; }

        /// <summary>
        /// Client status.
        /// </summary>
        public required UpdateClientStatusRequest Status { get; set; }

        /// <summary>
        /// Client right.
        /// </summary>
        public required UpdateClientRightRequest Right { get; set; }

        /// <summary>
        /// Flag indicating whether the client is for our system.
        /// </summary>
        public required bool IsInternal { get; set; }
    }
}