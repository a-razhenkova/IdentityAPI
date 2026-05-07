namespace Application.RabbitMq
{
    [RabbitMqEvent("login-from-new-ip-address")]
    public class LoginFromNewIpAddressEvent
    {
        public required long UserId { get; set; }

        public required string UserEmail { get; set; }

        public required string Username { get; set; }

        public required DateTime Timestamp { get; set; }

        public string? IpAddress { get; set; }
    }
}