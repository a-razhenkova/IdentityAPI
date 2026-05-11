namespace Application
{
    public static class UserOtpKey
    {
        private const int UserSecretLength = 16;

        public static string Create()
            => Pbkdf2KeySecret.CreateToBase64(UserSecretLength);
    }
}