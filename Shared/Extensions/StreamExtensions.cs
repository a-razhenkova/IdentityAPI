using System.Security.Cryptography;

namespace Shared
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadToEndAsync(this Stream value)
        {
            value.Position = 0;

            var streamReader = new StreamReader(value);
            string valueString = await streamReader.ReadToEndAsync();

            value.Position = 0;
            return valueString;
        }

        public static string ComputeMd5Checksum(this Stream stream)
        {
            if (stream is null || stream.Length == 0)
                throw new ArgumentException("No content.");

            using MD5 md5 = MD5.Create();

            stream.Position = 0;
            ReadOnlySpan<byte> hash = md5.ComputeHash(stream);

            if (hash.IsEmpty)
                throw new ArgumentException($"Failed to compute checksum.");

            return Convert.ToHexString(hash);
        }
    }
}