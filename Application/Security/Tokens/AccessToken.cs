using Domain;

namespace Application
{
    public class AccessToken : SecurityToken
    {
        public AccessToken(string token, SecuritySettings settings)
            : base(token, settings, settings.AccessToken.Key)
        {

        }

        public AccessToken(SecuritySettings options)
            : base(options, options.AccessToken.Key)
        {

        }

        public string Create(object type)
        {
            ISecurityToken token = type switch
            {
                Client client => new ClientAccessToken(client, _settings.AccessToken),
                User user => new UserAccessToken(user, _settings.AccessToken),
                _ => throw new NotImplementedException()
            };

            Value = new SecurityTokenHandler(token, _settings).Create();

            return Value;
        }
    }
}