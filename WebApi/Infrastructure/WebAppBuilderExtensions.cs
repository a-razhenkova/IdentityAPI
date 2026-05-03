using AutoMapper;
using Business;
using Database;
using Database.IdentityDb;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;

namespace WebApi
{
    public static class WebAppBuilderExtensions
    {
        public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
        {
            string identityDbConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.IdentityDb);
            string identityDbAddress = Regex.Match(identityDbConnectionString, @"^Server=[^;]+;Database=[^;]+;").Value;

            string redisConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis);
            string redisAddress = Regex.Match(redisConnectionString, @"^[^,]+").Value;

            string rabbitMqConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.RabbitMq);
            string rabbitMqAddress = Regex.Match(rabbitMqConnectionString, @"(?<=@)[^\/]+").Value;

            builder.Services.AddHealthChecks()
                            .AddDbContextCheck<IdentityDbContext>($"{ConnectionStringNames.IdentityDb}", tags: [HealthCheckImpactTag.Critical.ToString(), identityDbAddress])
                            .AddRedis(redisConnectionString, $"{ConnectionStringNames.Redis}", tags: [HealthCheckImpactTag.Medium.ToString(), redisAddress], timeout: TimeSpan.FromSeconds(2))
                            .AddCheck<RabbitMqHealthCheck>($"{ConnectionStringNames.RabbitMq}", tags: [HealthCheckImpactTag.Critical.ToString(), rabbitMqAddress], timeout: TimeSpan.FromSeconds(2));

            return builder;
        }

        public static async Task<WebApplicationBuilder> AddRabbitMqAsync(this WebApplicationBuilder builder)
        {
            string rabbitMqConnectionString = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.RabbitMq);

            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(2)
            };

            IConnection connection = await factory.CreateConnectionAsync();
            builder.Services.AddSingleton(connection);

            return builder;
        }

        public static MapperConfiguration CreateMapperConfig()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new IdentityDbMapperProfile());

                cfg.AddProfile(new V1.CommonMapperProfile());
                cfg.AddProfile(new V2.CommonMapperProfile());
            });

            mapperConfig.AssertConfigurationIsValid();

            return mapperConfig;
        }

        public static WebApplicationBuilder AddMapper(this WebApplicationBuilder builder)
        {
            MapperConfiguration mapperConfig = CreateMapperConfig();

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            return builder;
        }

        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            DatabaseAttribute identityDbConfig = typeof(IdentityDbContext).GetRequiredCustomAttribute<DatabaseAttribute>();
            builder.Services.AddDbContext<IdentityDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetRequiredConnectionString(identityDbConfig.ConnectionStringName), cfg =>
                {
                    cfg.CommandTimeout(identityDbConfig.CommandTimeoutInSeconds);
                    cfg.MigrationsAssembly(DatabaseAssembly.GetExecutingAssembly());
                    cfg.MigrationsHistoryTable(identityDbConfig.MigrationsHistoryTableName, identityDbConfig.DefaultSchemaName);
                });
#if DEBUG
                opt.LogTo(src => Debug.WriteLine(src));
                opt.EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();
#endif
            });

            return builder;
        }

        public static WebApplicationBuilder AddCache(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis));
            builder.Services.AddSingleton<IRedis, RedisService>();
            return builder;
        }

        public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
        {
            var securityOptions = builder.Configuration.GetRequiredSection<SecurityOptions>(nameof(AppSettingsOptions.Security));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(opt =>
                            {
                                opt.SaveToken = true;
                                opt.TokenValidationParameters = new AccessToken(securityOptions).ValidationParams;
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
            var securityOptions = builder.Configuration.GetRequiredSection<SecurityOptions>(nameof(AppSettingsOptions.Security));

            builder.Services.AddRateLimiter(opt =>
            {
                opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string? userIp = httpContext.GetUserIpAddress();

                    return RateLimitPartition.GetFixedWindowLimiter(!string.IsNullOrWhiteSpace(userIp) ? userIp : httpContext.Request.Headers.Host.ToString(),
                    partition => new FixedWindowRateLimiterOptions()
                    {
                        Window = TimeSpan.FromSeconds(securityOptions.RateLimiter.WindowInSeconds),
                        AutoReplenishment = true,
                        PermitLimit = securityOptions.RateLimiter.RequestsPerWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            return builder;
        }

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