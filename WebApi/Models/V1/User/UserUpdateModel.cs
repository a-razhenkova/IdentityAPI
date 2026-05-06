using Domain;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserUpdateModel
    {
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
        public required UserStatusModel Status { get; set; }
    }
}