namespace Application.RabbitMq
{
    [RabbitMqEvent("new-user-otp")]
    public class NewUserOtpEvent
    {
        public required long UserId { get; set; }

        public required string Otp { get; set; }
    }
}