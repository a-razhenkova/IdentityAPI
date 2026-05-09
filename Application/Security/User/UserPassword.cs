using Domain;

namespace Application
{
    public static class UserPassword
    {
        public const int Iterations = 100_000;
        public const int HashLength = 128;
        public const int SaltLength = 16;

        public static Domain.UserPassword Create(string password)
        {
            (string hash, string salt) = Pbkdf2Key.Create(password, Iterations, HashLength, SaltLength);

            return new Domain.UserPassword()
            {
                Value = hash,
                Secret = salt,
                LastChangedTimestamp = DateTime.UtcNow
            };
        }

        public static bool IsMatch(this Domain.UserPassword password, string originalPassword)
            => Pbkdf2Key.IsValid(password.Value, originalPassword, password.Secret, Iterations, HashLength);
    }
}