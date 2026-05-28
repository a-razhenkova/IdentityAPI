using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.BindConfigurationSources();
builder.AddLogger();

builder.AddServices();
builder.AddHealthChecks();
builder.AddResiliencePipelines();
builder.AddCache();
builder.AddDatabase();

builder.AddControllers();
builder.AddAuthentication();
builder.AddAuthorization();
builder.AddRateLimiter();

await builder.AddRabbitMqAsync();

builder.AddSwagger();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.MapControllers();
app.UseSwagger();

app.UseMiddleware<HttpMessageLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<HttpHeaderHandlingMiddleware>();

await app.ApplyDbPendingMigrationsAndScriptsAsync();

await app.RunAsync();