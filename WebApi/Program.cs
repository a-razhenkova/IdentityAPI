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
builder.AddSwagger();

await builder.AddRabbitMqAsync();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.UseSwagger();

app.UseMiddleware<HttpMessageLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<HttpHeaderHandlingMiddleware>();

await app.ApplyDbPendingMigrationsAndScriptsAsync();

await app.RunAsync();