using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class TestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.Tests.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureTestServices(services =>
            {
                RemoveRateLimiter(services);
            });
        }

        private static void RemoveRateLimiter(IServiceCollection services)
            => services.AddRateLimiter(opt => opt.GlobalLimiter = null);
    }
}