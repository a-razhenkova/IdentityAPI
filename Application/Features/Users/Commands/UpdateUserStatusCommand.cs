using Domain;

namespace Application
{
    public sealed record UpdateUserStatusCommand
    {
        public required UserStatuses Value { get; set; }

        public required UserStatusReasons Reason { get; set; }

        public string? Note { get; set; }
    }
}