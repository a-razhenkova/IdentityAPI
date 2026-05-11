using Application;
using AutoMapper;
using Infrastructure;
using Polly;
using Polly.Retry;
using Shared;

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
            builder.Services.AddScoped<IEmail, EmailService>();

            return builder;
        }

        public static MapperConfiguration CreateMapperConfig()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new V1.TokenProfile());
                cfg.AddProfile(new V1.ClientProfile());
                cfg.AddProfile(new V1.UserProfile());
                cfg.AddProfile(new V2.TokenProfile());
            });

            mapperConfig.AssertConfigurationIsValid();

            return mapperConfig;
        }

        public static WebApplicationBuilder AddResiliencePipelines(this WebApplicationBuilder builder)
        {
            builder.Services.AddResiliencePipeline(ResiliencePipelineType.RabbitMQ_PublishFastEvent, cfg =>
            {
                cfg.AddRetry(new RetryStrategyOptions
                {
                    BackoffType = DelayBackoffType.Constant,
                    UseJitter = false,
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                });
            });

            builder.Services.AddResiliencePipeline(ResiliencePipelineType.RabbitMQ_PublishEventInBackground, cfg =>
            {
                cfg.AddRetry(new RetryStrategyOptions
                {
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    MaxRetryAttempts = 10,
                    Delay = TimeSpan.FromSeconds(30),
                });
            });

            return builder;
        }
    }
}