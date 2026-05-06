namespace Shared
{
    public static class StringExtensions
    {
        public static bool BeginsWith(this string value, string substring)
            => value.IndexOf(substring) == 0;

        public static string TrimStart(this string value, string redundantValue)
        {
            if (value.Contains(redundantValue))
            {
                int startIndex = value.IndexOf(redundantValue);
                value = value.SubString(startIndex + redundantValue.Length, value.Length);
            }

            return value;
        }

        public static string SubString(this string value, int startIndex, int length, string defaultValue = "")
        {
            string substring = defaultValue;

            if (startIndex < value.Length)
            {
                if (startIndex + length > value.Length)
                    length = value.Length - startIndex;

                substring = value.Substring(startIndex, length);
            }

            return substring;
        }
    }
}