using Application;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Shared;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WebApi
{
    public static class ServicesSetup
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // external libraries
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper();

            // transient services
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            // scoped services
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

        public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
        {
            string identityDbConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.IdentityDb);
            string identityDbAddress = Regex.Match(identityDbConnectionString, @"^Server=[^;]+;Database=[^;]+;").Value;

            string redisConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis);
            string redisAddress = Regex.Match(redisConnectionString, @"^[^,]+").Value;

            string rabbitMqConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.RabbitMq);
            string rabbitMqAddress = Regex.Match(rabbitMqConnectionString, @"(?<=@)[^\/]+").Value;

            builder.Services.AddHealthChecks()
                            .AddDbContextCheck<IdentityContext>($"{ConnectionStringNames.IdentityDb}", tags: [HealthCheckImpactTag.Critical.ToString(), identityDbAddress])
                            .AddRedis(redisConnectionString, $"{ConnectionStringNames.Redis}", tags: [HealthCheckImpactTag.Medium.ToString(), redisAddress], timeout: TimeSpan.FromSeconds(2))
                            .AddCheck<RabbitMqHealthCheck>($"{ConnectionStringNames.RabbitMq}", tags: [HealthCheckImpactTag.Critical.ToString(), rabbitMqAddress], timeout: TimeSpan.FromSeconds(2));

            return builder;
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

        public static WebApplicationBuilder AddCache(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis));
            builder.Services.AddSingleton<IRedis, RedisService>();
            return builder;
        }

        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            DatabaseAttribute identityConfig = typeof(IdentityContext).GetRequiredCustomAttribute<DatabaseAttribute>();
            builder.Services.AddDbContext<IdentityContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetRequiredConnectionString(identityConfig.ConnectionStringName), cfg =>
                {
                    cfg.CommandTimeout(identityConfig.CommandTimeoutInSeconds);
                    cfg.MigrationsAssembly(InfrastructureAssembly.GetExecutingAssembly());
                    cfg.MigrationsHistoryTable(identityConfig.MigrationsHistoryTableName, identityConfig.DefaultSchemaName);
                });
#if DEBUG
                opt.LogTo(src => Debug.WriteLine(src));
                opt.EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();
#endif
            });

            return builder;
        }

        private static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new V1.TokenProfile());
                cfg.AddProfile(new V1.ClientProfile());
                cfg.AddProfile(new V1.UserProfile());
                cfg.AddProfile(new V2.TokenProfile());
            });

            mapperConfig.AssertConfigurationIsValid();

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}