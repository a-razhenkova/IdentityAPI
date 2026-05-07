using Application;
using Domain;

namespace Tests.Mocks
{
    public class ClientMocks
    {
        public static Client CreateBasicClient(
            string? name = default,
            string? key = default,
            string? secret = default,
            int wrongLoginAttemptsCounter = 0,
            bool isInternal = true,
            ClientStatuses clientStatus = ClientStatuses.Active,
            ClientStatusReasons clientStatusReason = ClientStatusReasons.None,
            bool canNotify = true)
        {
            return new Client()
            {
                Id = new Random().Next(100, 1000),
                Name = name,
                Key = string.IsNullOrWhiteSpace(key) ? ClientSecret.Create() : key,
                Secret = string.IsNullOrWhiteSpace(secret) ? ClientSecret.Create() : secret,
                WrongLoginAttemptsCounter = wrongLoginAttemptsCounter,
                IsInternal = isInternal,
                Status = new ClientStatus()
                {
                    Id = new Random().Next(100, 1000),
                    Value = clientStatus,
                    Reason = clientStatusReason
                },
                Right = new ClientRight()
                {
                    CanNotify = canNotify
                }
            };
        }
    }
}