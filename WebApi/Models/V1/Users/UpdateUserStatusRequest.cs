using Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UpdateUserStatusRequest
    {
        /// <summary>
        /// Status.
        /// </summary>
        public required UserStatuses Value { get; set; }

        /// <summary>
        /// Status reason.
        /// </summary>
        public required UserStatusReasons Reason { get; set; }

        /// <summary>
        /// Additional note for the status.
        /// </summary>
        /// <example>test</example>
        [RegularExpression(UserConstants.StatusNoteRegex)]
        public string? Note { get; set; }
    }
}