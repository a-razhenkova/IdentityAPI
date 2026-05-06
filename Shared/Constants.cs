namespace Shared
{
    public static class HttpHeaders
    {
        public const string Authorization = "Authorization";
        public const string RequestId = "X-Request-ID";
        public const string CorrelationId = "X-Correlation-ID";
        public const string ForwardedFor = "X-Forwarded-For";
    }

    public static class ConnectionStringNames
    {
        public const string IdentityDb = "IdentityDb";
        public const string Redis = "Redis";
        public const string RabbitMq = "RabbitMQ";
    }

    public static class Constants
    {
        public const string DefaultAssemblyVersion = "1.0.0.0";

        public const int UidLength = 36;
        public const int IpAddressMaxLength = 39;
        public const int OneTimePasswordLength = 6;

        public const string SerilogOutputTemplate = "> {Timestamp:dd.MM.yyyy HH:mm:ss.fff} - {Level:u3}|{MachineName}(v.{Version})|{SourceContext}{NewLine}[{Properties}]{NewLine}Message:{Message:lj}{NewLine}{Exception}";
        
        public const string FreeTextRegex = @"^[a-zA-Z\d\s\-.!?()]*$";
    }
}