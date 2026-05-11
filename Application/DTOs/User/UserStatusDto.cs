using Domain;

namespace Application
{
    public class UserStatusDto
    {
        public UserStatuses Value { get; set; } = UserStatuses.Active;

        public UserStatusReasons Reason { get; set; } = UserStatusReasons.None;

        public string? Note { get; set; }
    }
}