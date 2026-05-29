using Bogus;
using WebApi.V1;

namespace IntegrationTests
{
    public class CreateClientRequestFaker_V1 : Faker<CreateClientRequest>
    {
        public CreateClientRequestFaker_V1()
        {
            RuleFor(s => s.Name, f => f.Company.CompanyName());
            RuleFor(s => s.IsInternal, f => true);
            RuleFor(s => s.Right, (f, c) => new CreateClientRightRequest()
            {
                CanNotify = true
            });
        }
    }
}