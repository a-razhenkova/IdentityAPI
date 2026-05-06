namespace Shared
{
    public static class FileExtensions
    {
        public const string Pdf = ".pdf";

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