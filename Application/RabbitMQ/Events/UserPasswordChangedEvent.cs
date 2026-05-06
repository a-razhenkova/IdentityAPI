namespace Application.RabbitMq
{
    public class UserPasswordChangedEvent
    {
        public required long UserId { get; set; }

        public required string UserEmail { get; set; }

        public required DateTime Timestamp { get; set; }

        public string? UserIpAddress { get; set; }
    }
}