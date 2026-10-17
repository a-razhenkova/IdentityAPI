namespace Application
{
    public class FileDto
    {
        public required string Name { get; set; }

        public required byte[] Content { get; set; }
    }
}