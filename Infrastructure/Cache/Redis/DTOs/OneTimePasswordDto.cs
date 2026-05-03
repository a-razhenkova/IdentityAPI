namespace Infrastructure.Redis
{
    public class OneTimePasswordDto
    {
        public required string UserExternalId { get; set; }

        public required string Otp { get; set; }

        public required int WrongAuthAttemptsCounter { get; set; } = 0;
    }
}