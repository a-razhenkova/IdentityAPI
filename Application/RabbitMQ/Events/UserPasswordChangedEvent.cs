namespace Application.RabbitMq
{
    [RabbitMqEvent("user-password-changed")]
    public class UserPasswordChangedEvent : UserEventBase
    {
        public required DateTime Timestamp { get; set; }

        public string? UserIpAddress { get; set; }
    }
}