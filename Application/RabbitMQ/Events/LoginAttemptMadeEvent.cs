namespace Application.RabbitMq
{
    [RabbitMqEvent("login-attempt-made")]
    public class LoginAttemptMadeEvent
    {
        public required long UserId { get; set; }

        public required string UserEmail { get; set; }

        public required string Username { get; set; }

        public required DateTime Timestamp { get; set; }

        public string? IpAddress { get; set; }
    }
}