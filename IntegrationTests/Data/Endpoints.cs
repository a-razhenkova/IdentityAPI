namespace IntegrationTests
{
    public static class Endpoints
    {
        public const string Heartbeat = "/v1/health/heartbeat";
        public const string DeployInfo = "/v1/health";
        public const string HealthChecks = "/v1/health/checks";

        public const string Token_V1 = "/v1/token";
        public const string Token_V2 = "/v2/token";

        public const string TokenStatus_V1 = "/v1/token/status";

        public const string Clients_V1 = "/v1/clients";
    }
}