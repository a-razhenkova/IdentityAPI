using Application;
using Microsoft.EntityFrameworkCore;

namespace WebApi
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(RequestDelegate next,
                                           ILogger<ExceptionHandlingMiddleware> logger,
                                           IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (BadRequestException exception)
            {
                await HandleException(exception.StatusCode, "Bad Request", httpContext, exception);
            }
            catch (ForbiddenException exception)
            {
                await HandleException(exception.StatusCode, "Forbidden", httpContext, exception);
            }
            catch (UnauthorizedException exception)
            {
                await HandleException(exception.StatusCode, "Unauthorized", httpContext, exception);
            }
            catch (NotFoundException exception)
            {
                await HandleException(exception.StatusCode, "Not Found", httpContext, exception);
            }
            catch (ConflictException exception)
            {
                await HandleException(exception.StatusCode, "Conflict", httpContext, exception);
            }
            catch (HttpException exception)
            {
                httpContext.Response.StatusCode = exception.StatusCode;
            }
            catch (DbUpdateConcurrencyException)
            {
                await HandleException(StatusCodes.Status409Conflict, "Conflict", httpContext);
            }
            catch (Exception exception)
            {
                LogException(exception);
                await HandleException(StatusCodes.Status500InternalServerError, "Internal Server Error", httpContext, exception);
            }
        }

        private async Task HandleException(int statusCode, string title, HttpContext httpContext, Exception? exception = null)
        {
            try
            {
                var response = new V1.ExceptionResponse()
                {
                    Title = title,
                    Message = exception?.Message,
                    Details = _environment.IsDevelopment() ? exception?.StackTrace : null
                };

                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(response);
            }
            catch
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        private void LogException(Exception exception, string? message = null)
        {
            try
            {
                _logger.LogError(exception, message ?? exception.Message);
            }
            catch
            {
                // continue
            }
        }
    }
}