using Domain;

namespace Application
{
    public sealed record UpdateClientStatusCommand
    {
        public required ClientStatuses Value { get; set; }

        public required ClientStatusReasons Reason { get; set; }

        public string? Note { get; set; }
    }
}