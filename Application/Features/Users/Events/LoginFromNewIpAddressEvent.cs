namespace Application.RabbitMq
{
    [RabbitMqEvent("login-from-new-ip-address")]
    public class LoginFromNewIpAddressEvent : UserEventBase
    {
        public required DateTime Timestamp { get; set; }

        public string? IpAddress { get; set; }
    }
}