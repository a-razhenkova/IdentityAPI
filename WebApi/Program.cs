using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.BindConfigurationSources();

builder.AddLogger();
builder.AddCache();
builder.AddDatabase();

await builder.AddRabbitMqAsync();

builder.AddMapper();
builder.AddServices();
builder.AddHealthChecks();

builder.AddAuthentication();
builder.AddAuthorization();
builder.AddRateLimiter();

builder.AddControllers();
builder.AddSwagger();

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