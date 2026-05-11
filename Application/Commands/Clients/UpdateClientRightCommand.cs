namespace Application
{
    public sealed record UpdateClientRightCommand
    {
        public required bool CanNotify { get; set; }
    }
}