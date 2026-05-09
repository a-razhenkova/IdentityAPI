using Shared;

namespace Application
{
    public static class ClientKeyHandler
    {
        public static string Create()
            => Guid.NewGuid().ToString();

        public static bool IsValid(string key)
            => GuidExtensions.IsValidUid(key);
    }
}