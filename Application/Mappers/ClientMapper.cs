using Domain;

namespace Application
{
    public static class ClientMapper
    {
        public static ClientDto MapToDto(this Client client)
        {
            var dto = new ClientDto()
            {
                Key = client.Key,
                Name = client.Name,
                IsInternal = client.IsInternal,
                Status = new ClientStatusDto()
                {
                    Value = client.Status.Value,
                    Reason = client.Status.Reason
                },
                Right = new ClientRightDto()
                {
                    CanNotify = client.Right.CanNotify
                },
                Subscriptions = new List<ClientSubscriptionDto>()
            };

            foreach (var subscription in client.Subscriptions)
            {
                dto.Subscriptions.Add(new ClientSubscriptionDto()
                {
                    ExpirationDate = subscription.Subscription.ExpirationDate,
                    ContractId = subscription.Subscription.Contract.Id,
                    ContractName = subscription.Subscription.Contract.Name,
                });
            }

            return dto;
        }

        public static IEnumerable<ClientDto> MapToDto(this IEnumerable<Client> clients)
        {
            var dto = new List<ClientDto>();

            foreach (var Client in clients)
            {
                var ClientDto = Client.MapToDto();
                dto.Add(ClientDto);
            }

            return dto;
        }

        public static Client Map(this Client client, CreateClientCommand command)
        {
            client.Key = ClientKey.Create();
            client.Name = command.Name;
            client.IsInternal = command.IsInternal;
            client.Status = new ClientStatus()
            {
                Value = command.IsInternal ? ClientStatuses.Active : ClientStatuses.Disabled,
                Reason = command.IsInternal ? ClientStatusReasons.None : ClientStatusReasons.ExpiredSubscription
            };
            client.Right = new ClientRight()
            {
                CanNotify = command.Right.CanNotify
            };

            return client;
        }

        public static Client Map(this Client client, UpdateClientCommand command)
        {
            client.Name = command.Name;
            client.IsInternal = command.IsInternal;
            client.UpdateStatus(command.Status.Value, command.Status.Reason, command.Status.Note);
            client.Right.CanNotify = command.Right.CanNotify;
            return client;
        }
    }
}