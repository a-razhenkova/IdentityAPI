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
            catch (BadRequestException ex)
            {
                await HandleException(ex.StatusCode, "Bad Request", httpContext, ex);
            }
            catch (ForbiddenException ex)
            {
                await HandleException(ex.StatusCode, "Forbidden", httpContext, ex);
            }
            catch (UnauthorizedException ex)
            {
                await HandleException(ex.StatusCode, "Unauthorized", httpContext, ex);
            }
            catch (NotFoundException ex)
            {
                await HandleException(ex.StatusCode, "Not Found", httpContext, ex);
            }
            catch (ConflictException ex)
            {
                await HandleException(ex.StatusCode, "Conflict", httpContext, ex);
            }
            catch (HttpException ex)
            {
                httpContext.Response.StatusCode = ex.StatusCode;
            }
            catch (DbUpdateConcurrencyException)
            {
                await HandleException(StatusCodes.Status409Conflict, "Conflict", httpContext);
            }
            catch (Exception ex)
            {
                LogException(ex);
                await HandleException(StatusCodes.Status500InternalServerError, "Internal Server Error", httpContext, ex);
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