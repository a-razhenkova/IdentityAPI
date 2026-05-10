namespace Application.RabbitMq
{
    [RabbitMqEvent("email-verification")]
    public class EmailVerificationEvent
    {
        public required string VerificationToken { get; set; }
    }
}