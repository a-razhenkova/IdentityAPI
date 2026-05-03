using System.Security.Cryptography;
using System.Text;

namespace Business
{
    public static class Pbkdf2Key
    {
        public static (string Pbkdf2Key, string Secret) Create(string value, int interactions, int hashLength, int saltLength)
            => Create(value, interactions, hashLength, saltLength, secret: null);

        public static (string Pbkdf2Key, string Secret) Recreate(string value, string secret, int interactions, int hashLength)
            => Create(value, interactions, hashLength, secret);

        public static bool IsValid(string pbkdf2Key, string originalValue, string secret, int interactions, int hashLength)
        {
            string recreatePbkdf2Key = Recreate(originalValue, secret, interactions, hashLength).Pbkdf2Key;
            return pbkdf2Key.Equals(recreatePbkdf2Key);
        }

        private static (string Pbkdf2Key, string Secret) Create(string value, int interactions, int hashLength, string secret)
            => Create(value, interactions, hashLength, secret.Length, secret);

        private static (string Pbkdf2Key, string Secret) Create(string value, int interactions, int hashLength, int saltLength, string? secret = null)
        {
            if (string.IsNullOrWhiteSpace(value) || interactions <= 0 || hashLength <= 0)
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(secret) && saltLength < 0)
                throw new InvalidOperationException();

            byte[] salt = string.IsNullOrWhiteSpace(secret) ? Pbkdf2KeySecret.Create(saltLength) : Convert.FromBase64String(secret);
            ReadOnlySpan<byte> key = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(value), salt, interactions, HashAlgorithmName.SHA256, hashLength);

            return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
        }
    }
}