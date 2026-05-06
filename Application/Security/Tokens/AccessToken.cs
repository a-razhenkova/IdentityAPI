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

        public string Create<TCaller>(TCaller caller)
        {
            ISecurityToken token = caller switch
            {
                Client client => new ClientAccessToken(client, _settings.AccessToken),
                User user => new UserAccessToken(user, _settings.AccessToken),
                _ => throw new NotImplementedException()
            };

            return new SecurityTokenHandler(token, _settings).Create();
        }
    }
}