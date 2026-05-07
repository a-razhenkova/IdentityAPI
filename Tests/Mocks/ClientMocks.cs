using Application;
using Domain;

namespace Tests.Mocks
{
    public class ClientMock
    {
        public static Client CreateBasicClient(
            string? name = default,
            string? key = default,
            string? secret = default,
            int wrongLoginAttemptsCounter = 0,
            bool isInternal = true,
            ClientStatuses status = ClientStatuses.Active,
            ClientStatusReasons statusReason = ClientStatusReasons.None,
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
                    Value = status,
                    Reason = statusReason
                },
                Right = new ClientRight()
                {
                    CanNotify = canNotify
                }
            };
        }
    }
}