using Domain;

namespace Application
{
    public class RefreshToken : SecurityToken
    {
        public RefreshToken(string token, SecuritySettings settings)
            : base(token, settings, settings.RefreshToken.Key)
        {

        }

        public RefreshToken(SecuritySettings settings)
            : base(settings, settings.RefreshToken.Key)
        {

        }

        public string Create(object type)
        {
            ISecurityToken token = type switch
            {
                User user => new UserRefreshToken(user, _settings.RefreshToken),
                _ => throw new NotImplementedException()
            };

            Value = new SecurityTokenHandler(token, _settings).Create();

            return Value;
        }
    }
}