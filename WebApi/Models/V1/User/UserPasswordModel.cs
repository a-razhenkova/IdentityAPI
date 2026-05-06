using Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserPasswordModel
    {
        /// <summary>
        /// Old password.
        /// </summary>
        /// <example>m4A0?Edis66a</example>
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string OldPassword { get; set; }

        /// <summary>
        /// New password.
        /// </summary>
        /// <example>8ENy$0YV936k</example>
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string NewPassword { get; set; }
    }
}