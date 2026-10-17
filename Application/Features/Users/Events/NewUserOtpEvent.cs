namespace Application.RabbitMq
{
    [RabbitMqEvent("new-user-otp")]
    public class NewUserOtpEvent : UserEventBase
    {
        public required string Otp { get; set; }
    }
}