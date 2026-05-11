using Shared;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.V2
{
    public class UserCredentialsResponse
    {
        /// <summary>
        /// User external ID.
        /// </summary>
        /// <example>2a47a4fc-3d90-4ddb-a1ec-a664c0a8a2f3</example>
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        /// <summary>
        /// User external ID.
        /// </summary>
        /// <example>ivan.ivanov</example>
        [JsonPropertyName("username")]
        public required string Username { get; set; }

        /// <summary>
        /// One time password (OTP).
        /// </summary>
        /// <example>271967</example>
        [JsonPropertyName("oneTimePassword")]
        [StringLength(Constants.OneTimePasswordLength)]
        public required string OneTimePassword { get; set; }
    }
}