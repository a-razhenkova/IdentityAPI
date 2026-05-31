using Application;
using Serilog.Context;
using Shared;
using System.Diagnostics;
using System.Text.Json;

namespace WebApi
{
    public class HttpMessageLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpMessageLoggingMiddleware> _logger;

        public HttpMessageLoggingMiddleware(RequestDelegate next,
                                           ILogger<HttpMessageLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.GetEndpoint()?.Metadata?.GetMetadata<SkipLogAttribute>() is not null)
            {
                await _next(httpContext);
            }
            else
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.WebApi))
                using (LogContext.PushProperty(LoggerContextProperty.TraceId.ToString(), httpContext.GetTraceId()))
                using (LogContext.PushProperty(LoggerContextProperty.CorrelationId.ToString(), httpContext.GetCorrelationId()))
                {
                    httpContext.Request.EnableBuffering();
                    string? requestBody = await ReadBodyAsync(httpContext.Request.Body);

                    if (string.IsNullOrWhiteSpace(requestBody))
                        requestBody = null;

                    using Stream originalResponseBody = httpContext.Response.Body;
                    using var tempResponseBody = new MemoryStream();
                    httpContext.Response.Body = tempResponseBody;

                    Stopwatch requestDuration = Stopwatch.StartNew();
                    await _next(httpContext);
                    requestDuration.Stop();

                    string? responseBody = await ReadBodyAsync(httpContext.Response.Body);
                    await tempResponseBody.CopyToAsync(originalResponseBody);
                    httpContext.Response.Body = originalResponseBody;

                    if (string.IsNullOrWhiteSpace(responseBody))
                        responseBody = null;

                    LogAction(httpContext, requestBody, responseBody, requestDuration);
                }
            }
        }

        private void LogAction(HttpContext httpContext, string? requestBody, string? responseBody, Stopwatch requestDuration)
        {
            SensitiveDataAttribute? sensitiveData = httpContext.GetEndpoint()?.Metadata?.GetMetadata<SensitiveDataAttribute>();

            bool isSuccessStatusCode = httpContext.Response.StatusCode >= StatusCodes.Status200OK && httpContext.Response.StatusCode < StatusCodes.Status300MultipleChoices;
            bool isRequestSensitive = sensitiveData is not null && sensitiveData.IsRequestSensitive;
            bool isResponseSensitive = isSuccessStatusCode && sensitiveData is not null && sensitiveData.IsResponseSensitive;

            var action = new HttpAction()
            {
                StatusCode = httpContext.Response.StatusCode,
                Method = httpContext.Request.Method,
                Duration = requestDuration.ElapsedMilliseconds,
                FromIp = httpContext.GetIpAddresses(),
                User = GetUser(httpContext, requestBody),
                RequestData = isRequestSensitive ? null : requestBody,
                ResponseData = isResponseSensitive ? null : responseBody
            };

            if (isSuccessStatusCode)
            {
                _logger.LogInformation("Request finished successful: {@HttpAction}", action);
            }
            else
            {
                _logger.LogError("Request finished with error: {@HttpAction}", action);
            }
        }

        private static string? GetUser(HttpContext httpContext, string? requestBody)
        {
            string? user = null;

            try
            {
                string? authorization = httpContext.GetAuthorization();

                if (!string.IsNullOrWhiteSpace(authorization))
                {
                    user = httpContext.GetUser();
                }
                else if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    try
                    {
                        var credentials = JsonSerializer.Deserialize<V1.TokenRequest>(requestBody) ?? throw new ArgumentException();
                        user = credentials.Username;
                    }
                    catch
                    {
                        var credentials = JsonSerializer.Deserialize<V2.TokenRequest>(requestBody) ?? throw new ArgumentException();
                        user = credentials.Username;
                    }
                }
            }
            catch
            {
                // continue
            }

            return user;
        }

        private async Task<string> ReadBodyAsync(Stream body)
        {
            string bodyMsg = string.Empty;

            try
            {
                bodyMsg = await body.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(bodyMsg))
                    bodyMsg = bodyMsg.RemoveJsonFormatting();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return bodyMsg;
        }
    }
}