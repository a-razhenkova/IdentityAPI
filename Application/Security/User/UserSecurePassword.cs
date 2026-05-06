using Domain;

namespace Application
{
    public static class UserSecurePassword
    {
        public const int Interactions = 100_000;
        public const int HashLength = 128;
        public const int SaltLength = 16;

        public static UserPassword Create(string rawPassword)
        {
            (string hash, string salt) = Pbkdf2Key.Create(rawPassword, Interactions, HashLength, SaltLength);

            return new UserPassword()
            {
                Value = hash,
                Secret = salt,
                LastChangedTimestamp = DateTime.UtcNow
            };
        }

        public static bool IsValid(string password, string originalPassword, string salt)
            => Pbkdf2Key.IsValid(password, originalPassword, salt, Interactions, HashLength);
    }
}