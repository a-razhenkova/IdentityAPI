using Shared;

namespace Application
{
    public static class UserOtpKey
    {
        private const int UserSecretLength = 16;

        public static string Create()
            => new RandomKey(UserSecretLength).CreateToBase64();
    }
}