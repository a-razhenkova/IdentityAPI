using Application;
using Infrastructure;

namespace WebApi
{
    public static class ServicesSetup
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // external libraries
            builder.Services.AddHttpContextAccessor();

            // transient services
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            // scoped services
            builder.Services.AddScoped<IHealthChecker, HealthCheckService>();
            builder.Services.AddScoped<IPaginatedReport, PaginatedReportService>();
            builder.Services.AddScoped<IRabbitMq, RabbitMqService>();

            builder.Services.AddScoped<IBasicAuthenticator, BasicAuthenticationService>();
            builder.Services.AddScoped<IBearerAuthenticator, BearerAuthenticationService>();
            builder.Services.AddScoped<IOtpAuthenticator, OtpAuthenticationService>();
            builder.Services.AddScoped<IClientAuthenticator, ClientAuthenticationService>();
            builder.Services.AddScoped<IUserAuthenticator, UserAuthenticationService>();

            builder.Services.AddScoped<IToken, TokenService>();
            builder.Services.AddScoped<IOtp, OtpService>();
            builder.Services.AddScoped<IClient, ClientService>();
            builder.Services.AddScoped<IUser, UserService>();

            return builder;
        }
    }
}