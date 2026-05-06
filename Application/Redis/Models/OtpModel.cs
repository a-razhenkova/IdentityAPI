namespace Application.Redis
{
    public class OtpModel
    {
        public required string UserPublicId { get; set; }

        public required string Otp { get; set; }

        public required int WrongAuthAttemptsCounter { get; set; } = 0;
    }
}