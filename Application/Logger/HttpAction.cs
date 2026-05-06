namespace Application
{
    public class HttpAction
    {
        public required int StatusCode { get; set; }

        public required string Method { get; set; }

        public required double Duration { get; set; }

        public string? FromIp { get; set; }

        public string? User { get; set; }

        public string? RequestData { get; set; }

        public string? ResponseData { get; set; }
    }
}