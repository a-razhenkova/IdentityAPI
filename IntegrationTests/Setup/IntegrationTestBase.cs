using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using System.Data.Common;
using WebApi;
using V1 = WebApi.V1;

namespace IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public class IntegrationTestCollection : ICollectionFixture<TestFactory> { }

    [Collection(nameof(IntegrationTestCollection))]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly TestFactory _factory;
        private Respawner _respawner = null!;
        protected TestClient _client = null!;

        protected IntegrationTestBase(TestFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();
            await InitIdentityContextAsync();
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            await ResetIdentityContextAsync();
        }

        public async Task SetAccessTokenByClientCredentialsAsync()
        {
            _client.SetBasicAuthorization(TestData.ClientKey, TestData.ClientSecret);

            var response = await _client.PostAsync<V1.TokenResponse>(Endpoints.Token_V1)
                ?? throw new ArgumentNullException();

            _client.SetBearerAuthorization(response.AccessToken);
        }

        public async Task SetAccessTokenByUserCredentialsAsync()
        {
            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = TestData.UserPassword
            };

            var response = await _client.PostAsync<V1.TokenRequest, V1.TokenResponse>(request, Endpoints.Token_V2)
                ?? throw new ArgumentNullException();

            _client.SetBearerAuthorization(response.AccessToken);
        }

        private async Task InitIdentityContextAsync()
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