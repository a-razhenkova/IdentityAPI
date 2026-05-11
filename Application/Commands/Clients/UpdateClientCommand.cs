namespace Application
{
    public sealed record UpdateClientCommand
    {
        public required string Name { get; set; }

        public required UpdateClientStatusCommand Status { get; set; }

        public required UpdateClientRightCommand Right { get; set; }

        public required bool IsInternal { get; set; }
    }
}