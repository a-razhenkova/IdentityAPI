using Shared;

namespace Application
{
    public static class ClientSecretHandler
    {
        public static string Create()
            => Guid.NewGuid().ToString();

        public static bool IsValid(string secret)
            => GuidExtensions.IsValidUid(secret);
    }
}