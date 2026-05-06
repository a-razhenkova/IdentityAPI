using System.Text.RegularExpressions;

namespace Shared
{
    public static class GuidExtensions
    {
        public static bool IsValidUid(string uid)
        {
            var regex = new Regex(@"^[\da-fA-F]{8}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{12}$", RegexOptions.IgnoreCase);
            return regex.Match(uid).Success;
        }
    }
}