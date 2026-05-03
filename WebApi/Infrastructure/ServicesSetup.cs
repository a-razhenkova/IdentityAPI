using Business;
using Infrastructure;

namespace WebApi
{
    public static class ServicesSetup
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // external libraries
            builder.Services.AddHttpContextAccessor();

            // scoped services
            builder.Services.AddScoped<IHealthChecker, HealthCheckService>();

            builder.Services.AddScoped<IBasicAuthenticator, BasicAuthenticationService>();
            builder.Services.AddScoped<IBearerAuthenticator, BearerAuthenticationService>();
            builder.Services.AddScoped<IOtpAuthenticator, OtpAuthenticationService>();
            builder.Services.AddScoped<IClientAuthenticator, ClientAuthenticationService>();
            builder.Services.AddScoped<IUserAuthenticator, UserAuthenticationService>();

            builder.Services.AddScoped<IReportHandler, ReportService>();
            builder.Services.AddScoped<ITokenHandler, TokenService>();
            builder.Services.AddScoped<IAlert, AlertService>();

            builder.Services.AddScoped<IClientHandler, ClientService>();
            builder.Services.AddScoped<IUserHandler, UserService>();

            return builder;
        }
    }
}