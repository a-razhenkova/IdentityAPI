using Microsoft.AspNetCore.Http;

namespace Application
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string? message = null) 
            : base(StatusCodes.Status401Unauthorized, message)
        {

        }
    }
}