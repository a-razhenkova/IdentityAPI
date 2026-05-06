using Domain;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserRegistrationModel
    {
        /// <summary>
        /// Username.
        /// </summary>
        /// <example>ivan.ivanov</example>
        [StringLength(UserConstants.UsernameMaxLength, MinimumLength = UserConstants.UsernameMinLength)]
        [RegularExpression(UserConstants.UsernameRegex)]
        public required string Username { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        /// <example>m4A0?Edis66a</example>
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string Password { get; set; }

        /// <summary>
        /// User role.
        /// </summary>
        public required UserRoles Role { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        [EmailAddress]
        public string? Email { get; set; }
    }
}