using Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.V1
{
    public class CreateOtpRequest
    {
        /// <summary>
        /// Username.
        /// </summary>
        /// <example>ivan.ivanov</example>
        [JsonPropertyName("username")]
        [StringLength(UserConstants.UsernameMaxLength, MinimumLength = UserConstants.UsernameMinLength)]
        [RegularExpression(UserConstants.UsernameRegex)]
        public required string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        /// <example>m4A0?Edis66a</example>
        [JsonPropertyName("password")]
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string Password { get; set; }
    }
}