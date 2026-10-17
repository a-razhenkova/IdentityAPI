namespace Application
{
    public class AesEncryptedFileDto
    {
        public required string Checksum { get; set; }

        public required string Key { get; set; }

        public required string Secret { get; set; }
    }
}