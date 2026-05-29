using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Data.Common;

namespace IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public class IntegrationTestCollection : ICollectionFixture<TestFactory> { }

    [Collection(nameof(IntegrationTestCollection))]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly TestFactory _factory;
        private DbConnection _dbConnection = null!;
        private Respawner _respawner = null!;

        protected IntegrationTestBase(TestFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await InitDatabase();
        }

        public async Task DisposeAsync()
        {
            await _dbConnection.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        public HttpClient CreateClient()
            => TestFactoryClient.Create(_factory);

        private async Task InitDatabase()
        {
            using var scope = _factory.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            _dbConnection = context.Database.GetDbConnection();

            await _dbConnection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                SchemasToInclude = ["dbo"],
                TablesToIgnore = ["applied_migration", "applied_script"]
            });
        }
    }
}