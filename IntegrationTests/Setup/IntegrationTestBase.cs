using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using System.Data.Common;
using WebApi;

namespace IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public class IntegrationTestCollection : ICollectionFixture<TestFactory> { }

    [Collection(nameof(IntegrationTestCollection))]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly TestFactory _factory;
        private Respawner _respawner = null!;
        protected HttpClient _client = null!;

        protected IntegrationTestBase(TestFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _client = TestFactoryClient.Create(_factory);
            await InitIdentityContext();
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            await ResetIdentityContextAsync();
        }

        private async Task InitIdentityContext()
        {
            using IServiceScope scope = _factory.Services.CreateScope();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            using DbConnection dbConnection = identityContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();

            _respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
            {
                SchemasToInclude = ["dbo"],
                TablesToIgnore = ["applied_migration"]
            });
        }

        private async Task ResetIdentityContextAsync()
        {
            using IServiceScope scope = _factory.Services.CreateScope();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            using DbConnection dbConnection = identityContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();

            await _respawner.ResetAsync(dbConnection);

            identityContext.ApplyDbPendingScriptsAsync(logger);
        }
    }
}