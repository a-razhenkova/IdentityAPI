namespace Application
{
    public class HttpException : Exception
    {
        public HttpException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpException(int statusCode, string? message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(int statusCode, Exception? exception)
            : base(exception?.Message, exception)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}