using Microsoft.AspNetCore.Http;

namespace Application
{
    public class UnprocessableContentException : HttpException
    {
        public UnprocessableContentException(string? message = null)
            : base(StatusCodes.Status422UnprocessableEntity, message)
        {

        }
    }
}