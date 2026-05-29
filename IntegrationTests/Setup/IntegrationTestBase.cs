using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected IntegrationTestBase() { }

        protected static HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }
    }
}