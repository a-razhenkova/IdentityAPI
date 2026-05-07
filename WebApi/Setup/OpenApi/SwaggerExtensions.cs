using Infrastructure;
using Microsoft.OpenApi;
using Shared;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi
{
    public static class SwaggerExtensions
    {
        public static WebApplication UseSwagger(this WebApplication app)
        {
            if (app.Environment.IsSwaggerAllowed())
            {
                app.UseSwagger(cfg =>
                {
                    cfg.RouteTemplate = "api/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/v1/swagger.json", "v1");
                    c.RoutePrefix = "api";
                });
            }

            return app;
        }

        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsSwaggerAllowed())
            {
                builder.Services.AddSwaggerGen(opt =>
                {
                    opt.UseUniqueIds();
                    opt.AddAuthorization();

                    opt.AddHeaders();
                    opt.AddXmlComments();
                    opt.AddSummaryAndContact();
                });
            }

            return builder;
        }

        private static bool IsSwaggerAllowed(this IWebHostEnvironment environment)
        {
            return environment.IsDevelopment() || environment.IsStaging();
        }

        private static void UseUniqueIds(this SwaggerGenOptions opt)
        {
            opt.OperationFilter<SwaggerOperationIdFilter>();

            opt.CustomSchemaIds(modelType =>
            {
                string schemaId = modelType.DefaultSchemaIdSelector();
                string? suffix = null;

                string fullType = modelType.ToString();
                if (fullType.Contains($"{nameof(WebApi)}."))
                {
                    if (fullType.Contains(".V1."))
                    {
                        suffix += "V1";
                    }
                    else if (fullType.Contains(".V2."))
                    {
                        suffix += "V2";
                    }
                    else if (fullType.Contains(".V3."))
                    {
                        suffix += "V3";
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                if (!string.IsNullOrWhiteSpace(suffix))
                    schemaId = $"{schemaId}_{suffix}";

                return schemaId;
            });
        }

        private static void AddAuthorization(this SwaggerGenOptions opt)
        {
            opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(AuthorizationSchema.Basic.ToString(), document)] = [],
                [new OpenApiSecuritySchemeReference(AuthorizationSchema.Bearer.ToString(), document)] = []
            });

            opt.AddSecurityDefinition(AuthorizationSchema.Basic.ToString(), new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Description = "Use to obtain access token for a client.",
                Scheme = OpenApiConstants.Basic
            });

            opt.AddSecurityDefinition(AuthorizationSchema.Bearer.ToString(), new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Description = "Adds 'Authorization' header with the value for all HTTP requests below.",
                Name = HttpHeaders.Authorization,
                In = ParameterLocation.Header,
                Scheme = OpenApiConstants.Bearer
            });
        }

        private static void AddHeaders(this SwaggerGenOptions opt)
        {
            opt.OperationFilter<SwaggerCorrelationIdFilter>();
        }

        private static void AddXmlComments(this SwaggerGenOptions opt)
        {
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{WebApiAssembly.GetName()}.xml"));
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{ApplicationAssembly.GetName()}.xml"));
        }

        private static void AddSummaryAndContact(this SwaggerGenOptions opt)
        {
            opt.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Identity API",
                Description =
@"<p>The API provides secure identity and access management. It supports <strong>user</strong> and <strong>client</strong> registration, update, deletion and retrieval with paginated report.</p>
<ul>
    <li>Use <code>POST /api/v1/token</code> for basic authorization to obtain <strong>access token</strong> for a <strong>client</strong>.</li>
    </br>
    <li>Use <code>POST /api/v2/token</code> for bearer authorization to obtain <strong>access and refresh token</strong> for an <strong>user</strong>. Use <code>PUT /api/v2/token</code> to refresh the <strong>access token</strong>.</li>
    </br>
    <li>Use <code>POST /api/v1/otp</code> to obtain <strong>one time password</strong> and then use it with <code>POST /api/v3/token</code> to obtain <strong>access and refresh token</strong> for a <strong>user</strong>.</li>
    </br>
    <li>Use <code>POST /api/v1/token/status</code> to validate <strong>access token</strong> for both clients and users.</li>
</ul>
<p><strong>Notes:</strong></p>
<p>To authenticate, external clients are required to activate a subscription via <code>POST /api/v1/clients/{key}/subscriptions</code>.<p>
<p>To authenticate with OTP, users are required to verify their email via <code>POST /api/v1/email/verification/{token}</code>.</p>",
                Version = $"{WebApiAssembly.GetVersion()}",
                Contact = new OpenApiContact()
                {
                    Name = "Aleksandrina Razhenkova",
                    Url = new Uri("https://github.com/a-razhenkova"),
                    Email = "a.razhenkova@gmail.com"
                }
            });
        }

        /// <summary>
        /// Same as the one in <see cref="SchemaGeneratorOptions"/>.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private static string DefaultSchemaIdSelector(this Type modelType)
        {
            if (!modelType.IsConstructedGenericType) return modelType.Name.Replace("[]", "Array");

            var prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }
    }
}