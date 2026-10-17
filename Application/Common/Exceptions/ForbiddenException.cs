using Microsoft.AspNetCore.Http;

namespace Application
{
    public class ForbiddenException : HttpException
    {
        public ForbiddenException(string? message = null)
            : base(StatusCodes.Status403Forbidden, message)
        {

        }
    }
}