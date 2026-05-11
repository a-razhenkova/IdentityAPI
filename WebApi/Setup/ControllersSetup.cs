using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace WebApi
{
    public static class ControllersSetup
    {
        public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                            .AddJsonOptions(opt =>
                            {
                                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
                            })
                            .ConfigureApiBehaviorOptions(opt =>
                            {
                                opt.InvalidModelStateResponseFactory = actionContext => CreateInvalidModelResponse(actionContext);
                            });
            builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

            return builder;
        }

        public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
        {
            var securitySettings = builder.Configuration.GetRequiredSection<SecuritySettings>(nameof(AppSettings.Security));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(opt =>
                            {
                                opt.SaveToken = true;
                                opt.TokenValidationParameters = new AccessToken(securitySettings).ValidationParams;
                            });

            return builder;
        }

        public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
            builder.Services.AddAuthorization();
            return builder;
        }

        public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
        {
            var securitySettings = builder.Configuration.GetRequiredSection<SecuritySettings>(nameof(AppSettings.Security));

            builder.Services.AddRateLimiter(opt =>
            {
                opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string? userIp = httpContext.GetUserIpAddress();

                    return RateLimitPartition.GetFixedWindowLimiter(!string.IsNullOrWhiteSpace(userIp) ? userIp : httpContext.Request.Headers.Host.ToString(),
                    partition => new FixedWindowRateLimiterOptions()
                    {
                        Window = TimeSpan.FromSeconds(securitySettings.RateLimiter.WindowInSeconds),
                        AutoReplenishment = true,
                        PermitLimit = securitySettings.RateLimiter.RequestsPerWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            return builder;
        }

        private static IActionResult CreateInvalidModelResponse(ActionContext context)
        {
            var message = new StringBuilder();

            foreach (KeyValuePair<string, ModelStateEntry> entry in context.ModelState)
            {
                if (message.Length > 0)
                    message.AppendLine();

                foreach (var error in entry.Value.Errors)
                    message.Append(error.ErrorMessage);
            }

            throw new BadRequestException(new Exception(message.ToString()));
        }
    }
}