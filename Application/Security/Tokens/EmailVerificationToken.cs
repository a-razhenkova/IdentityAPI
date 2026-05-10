using Domain;

namespace Application
{
    public class EmailVerificationToken : SecurityToken
    {
        public EmailVerificationToken(string token, SecuritySettings settings)
            : base(token, settings, settings.EmailVerificationToken.Key)
        {

        }

        public EmailVerificationToken(SecuritySettings options)
            : base(options, options.EmailVerificationToken.Key)
        {

        }

        public string Create(User user)
        {
            var token = new UserEmailVerificationToken(user, _settings.EmailVerificationToken);
            Value = new SecurityTokenHandler(token, _settings).Create();
            return Value;
        }
    }
}