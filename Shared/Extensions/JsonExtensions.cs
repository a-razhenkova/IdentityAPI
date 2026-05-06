namespace Shared
{
    public static class JsonExtensions
    {
        public static string RemoveJsonFormatting(this string json)
        {
            return json.Replace("\n", string.Empty)
                       .Replace("\r", string.Empty)
                       .Replace(" ", string.Empty);
        }
    }
}