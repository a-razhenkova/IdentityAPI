using System.Security.Cryptography;
using System.Text;

namespace Application
{
    public static class Pbkdf2Key
    {
        public static (string Pbkdf2Key, string Secret) Create(string value, int iterations, int hashLength, int saltLength)
            => Create(value, iterations, hashLength, saltLength, secret: null);

        public static (string Pbkdf2Key, string Secret) Recreate(string value, string secret, int iterations, int hashLength)
            => Create(value, iterations, hashLength, secret);

        public static bool IsValid(string pbkdf2Key, string originalValue, string secret, int iterations, int hashLength)
        {
            string recreatePbkdf2Key = Recreate(originalValue, secret, iterations, hashLength).Pbkdf2Key;
            return pbkdf2Key.Equals(recreatePbkdf2Key);
        }

        private static (string Pbkdf2Key, string Secret) Create(string value, int iterations, int hashLength, string secret)
            => Create(value, iterations, hashLength, secret.Length, secret);

        private static (string Pbkdf2Key, string Secret) Create(string value, int iterations, int hashLength, int saltLength, string? secret)
        {
            if (string.IsNullOrWhiteSpace(value) || iterations <= 0 || hashLength <= 0)
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(secret) && saltLength < 0)
                throw new InvalidOperationException();

            byte[] salt = string.IsNullOrWhiteSpace(secret) ? Pbkdf2KeySecret.Create(saltLength) : Convert.FromBase64String(secret);
            ReadOnlySpan<byte> key = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(value), salt, iterations, HashAlgorithmName.SHA256, hashLength);

            return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
        }
    }
}