namespace Application.RabbitMq
{
    [RabbitMqEvent("login-attempt-made")]
    public class LoginAttemptMadeEvent : UserEventBase
    {
        public required DateTime Timestamp { get; set; }

        public string? IpAddress { get; set; }
    }
}