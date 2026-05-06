using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace Shared
{
    public static class HttpContextExtensions
    {
        public static string GetTraceId(this HttpContext httpContext)
            => Activity.Current?.Id ?? httpContext.TraceIdentifier ?? Guid.NewGuid().ToString();

        public static string GetBaseAddress(this HttpContext httpContext)
            => $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";

        public static string GetCorrelationId(this HttpContext httpContext)
        {
            string? correlationId = null;

            if (httpContext.Request.Headers.TryGetValue(HttpHeaders.CorrelationId, out StringValues value))
            {
                correlationId = value.FirstOrDefault() ?? string.Empty;
            }

            return string.IsNullOrWhiteSpace(correlationId) ? GetTraceId(httpContext) : correlationId;
        }

        public static string? GetAuthorization(this HttpContext httpContext)
        {
            string? authorization = null;

            if (httpContext.Request.Headers.TryGetValue(HttpHeaders.Authorization, out StringValues value))
            {
                authorization = value.FirstOrDefault() ?? string.Empty;
            }

            return authorization;
        }

        public static string? GetIpAddresses(this HttpContext httpContext)
        {
            string? ipAddress = null;

            if (httpContext.Request.Headers.TryGetValue(HttpHeaders.ForwardedFor, out StringValues forwardedFor))
            {
                ipAddress = forwardedFor.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = httpContext.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            }

            return ipAddress;
        }

        public static string? GetUserIpAddress(this HttpContext httpContext)
        {
            string? ipAddress = null;

            if (httpContext.Request.Headers.TryGetValue(HttpHeaders.ForwardedFor, out StringValues forwardedFor))
            {
                ipAddress = forwardedFor.ToString();

                string[] ipAddresses = ipAddress.Split(',');
                ipAddress = ipAddresses[ipAddresses.Length - 1].Trim();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = httpContext.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            }

            return ipAddress;
        }
    }
}