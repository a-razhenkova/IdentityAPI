using Microsoft.AspNetCore.Http;

namespace Application
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string? message = null)
            : base(StatusCodes.Status404NotFound, message)
        {

        }
    }
}