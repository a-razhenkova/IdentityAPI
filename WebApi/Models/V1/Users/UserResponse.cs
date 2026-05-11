using Domain;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserResponse
    {
        /// <summary>
        /// User external ID.
        /// </summary>
        /// <example>2a47a4fc-3d90-4ddb-a1ec-a664c0a8a2f3</example>
        public required string Id { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        /// <example>ivan.ivanov</example>
        [StringLength(UserConstants.UsernameMaxLength, MinimumLength = UserConstants.UsernameMinLength)]
        [RegularExpression(UserConstants.UsernameRegex)]
        public required string Username { get; set; }

        /// <summary>
        /// User role.
        /// </summary>
        public required UserRoles Role { get; set; }

        /// <summary>
        /// User status.
        /// </summary>
        public required UserStatusResponse Status { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        /// <example>ivan.ivanov@mail.com</example>
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Registration timestamp.
        /// </summary>
        public required DateTime RegistrationTimestamp { get; set; }
    }
}