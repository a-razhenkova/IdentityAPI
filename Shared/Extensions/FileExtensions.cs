using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Shared
{
    public static class FileExtensions
    {
        public const string Pdf = ".pdf";

        public static async Task<AesEncryptedFile> CreateAndAesEncryptAsync(this IFormFile file, string filePath, CancellationToken cancellationToken = default)
        {
            AesEncryptedFile encryptedFile;

            EnsureFileDirectoryExists(filePath);

            try
            {
                using Stream content = File.Create(filePath);

                using var aes = Aes.Create();
                aes.Key = new RandomKey(Constants.Aes256KeySize).Create();
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor();
                using var encryptedStream = new CryptoStream(content, encryptor, CryptoStreamMode.Write);

                await file.CopyToAsync(encryptedStream, cancellationToken);

                encryptedFile = new AesEncryptedFile()
                {
                    Checksum = content.ComputeMd5Checksum(),
                    Key = Convert.ToBase64String(aes.Key),
                    Secret = Convert.ToBase64String(aes.IV)
                };
            }
            catch
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                throw;
            }

            return encryptedFile;
        }

        public static async Task<byte[]> ReadAndAesDecryptAllBytesAsync(string filePath, string key, string secret, CancellationToken cancellationToken = default)
        {
            using Stream content = File.OpenRead(filePath);

            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(secret);

            using var decryptor = aes.CreateDecryptor();
            using var decryptedStream = new CryptoStream(content, decryptor, CryptoStreamMode.Read);

            using var decryptedContent = new MemoryStream();
            await decryptedStream.CopyToAsync(decryptedContent, cancellationToken);

            return decryptedContent.ToArray();
        }

        public static void EnsureFileDirectoryExists(string filePath, bool canCreateDirectoryIfNotFound = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid path.");

            string fileDirectory = Path.GetDirectoryName(filePath)
                ?? throw new InvalidOperationException();

            EnsureDirectoryExists(fileDirectory, canCreateDirectoryIfNotFound);
        }

        public static void EnsureDirectoryExists(string directoryPath, bool canCreateDirectoryIfNotFound = true)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Invalid path.");

            if (!Directory.Exists(directoryPath))
            {
                if (canCreateDirectoryIfNotFound)
                {
                    Directory.CreateDirectory(directoryPath);
                }
                else
                {
                    throw new DirectoryNotFoundException($"Directory '{directoryPath}' not found.");
                }
            }
        }
    }
}