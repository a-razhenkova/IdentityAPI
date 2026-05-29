using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests
{
    public static class TestFactoryClient
    {
        private static WebApplicationFactoryClientOptions _factoryOptions = new()
        {
            BaseAddress = new Uri("https://localhost/")
        };

        public static HttpClient Create()
            => Create(new TestFactory());

        public static HttpClient Create(TestFactory factory)
            => factory.CreateClient(_factoryOptions);
    }
}