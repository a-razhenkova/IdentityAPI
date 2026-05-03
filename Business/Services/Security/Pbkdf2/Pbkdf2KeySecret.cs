using System.Security.Cryptography;

namespace Business
{
    public static class Pbkdf2KeySecret
    {
        public static byte[] Create(int size)
        {
            var secret = new byte[size];
            RandomNumberGenerator.Fill(secret);
            return secret;
        }

        public static string CreateToBase64(int size)
        {
            ReadOnlySpan<byte> secret = Create(size);
            return Convert.ToBase64String(secret);
        }
    }
}