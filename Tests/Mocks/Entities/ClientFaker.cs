using Application;
using Bogus;
using Domain;

namespace Tests.Mocks
{
    public class ClientFaker : Faker<Client>
    {
        public ClientFaker()
        {
            RuleFor(s => s.Id, f => f.Random.Long());
            RuleFor(s => s.Name, f => f.Company.CompanyName());
            RuleFor(s => s.Key, f => ClientSecretHandler.Create());
            RuleFor(s => s.WrongLoginAttemptsCounter, f => f.Random.Int(0, 2));
            RuleFor(s => s.IsInternal, f => true);
            RuleFor(s => s.Status, (f, c) => new ClientStatus()
            {
                Id = f.Random.Long(),
                ClientId = c.Id,
                Value = ClientStatuses.Active,
                Reason = ClientStatusReasons.None,
                Note = f.Random.String(),
                Client = c
            });
            RuleFor(s => s.Right, (f, c) => new ClientRight()
            {
                Id = f.Random.Long(),
                ClientId = c.Id,
                CanNotify = true,
                Client = c
            });
        }
    }
}