using Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class SearchUserRequest : SearchRequest
    {
        /// <summary>
        /// Username.
        /// </summary>
        [StringLength(UserConstants.UsernameMaxLength)]
        public string? Username { get; set; }

        /// <summary>  
        /// User role.  
        /// </summary>
        public UserRoles? Role { get; set; }

        /// <summary>
        /// User status.
        /// </summary>
        public UserStatuses? Status { get; set; }
    }
}