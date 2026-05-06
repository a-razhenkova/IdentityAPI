using Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class ClientRegistrationModel
    {
        /// <summary>
        /// Name.
        /// </summary>
        /// <example>Auth API</example>
        [StringLength(ClientConstants.Name)]
        [RegularExpression(ClientConstants.NameRegex)]
        public required string Name { get; set; }

        /// <summary>
        /// Client right.
        /// </summary>
        public required ClientRightModel Right { get; set; }

        /// <summary>
        /// Flag indicating whether the client is for our system.
        /// </summary>
        public required bool IsInternal { get; set; }
    }
}