namespace Application
{
    public sealed record CreateClientRightCommand
    {
        public required bool CanNotify { get; set; }
    }
}