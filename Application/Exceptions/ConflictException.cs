using Microsoft.AspNetCore.Http;

namespace Application
{
    public class ConflictException : HttpException
    {
        public ConflictException(string? message = null)
            : base(StatusCodes.Status409Conflict, message)
        {

        }
    }
}