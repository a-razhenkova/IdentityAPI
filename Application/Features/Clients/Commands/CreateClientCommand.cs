namespace Application
{
    public sealed record CreateClientCommand
    {
        public required string Name { get; set; }

        public required bool IsInternal { get; set; }

        public required CreateClientRightCommand Right { get; set; }
    }
}