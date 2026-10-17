using Domain;

namespace Application
{
    public class UserStatusDto
    {
        public required UserStatuses Value { get; set; }

        public required UserStatusReasons Reason { get; set; }

        public string? Note { get; set; }
    }
}