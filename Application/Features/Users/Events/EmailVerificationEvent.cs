namespace Application.RabbitMq
{
    [RabbitMqEvent("email-verification")]
    public class EmailVerificationEvent : UserEventBase
    {
        public required string VerificationToken { get; set; }
    }
}