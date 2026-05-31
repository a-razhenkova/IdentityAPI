using Application;
using Shared;

namespace WebApi
{
    public class HttpHeaderHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpHeaderHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Response.Headers[HttpHeaders.RequestId] = httpContext.GetTraceId();
            httpContext.Response.Headers[HttpHeaders.CorrelationId] = httpContext.GetCorrelationId();

            await _next(httpContext);
        }
    }
}