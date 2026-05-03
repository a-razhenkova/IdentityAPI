using Database.IdentityDb.DefaultSchema;

namespace Business
{
    public class OtpService : IOtp
    {
        private readonly IBearerAuthenticator _bearerAuthenticator;
        private readonly IOtpAuthenticator _otpAuthenticator;

        public OtpService(IBearerAuthenticator bearerAuthenticator,
                         IOtpAuthenticator otpAuthenticator)
        {
            _bearerAuthenticator = bearerAuthenticator;
            _otpAuthenticator = otpAuthenticator;
        }

        public async Task<string> CreateAndSendAsync(string username, string password)
        {
            User user = await _bearerAuthenticator.AuthenticateAsync(username, password);
            return await _otpAuthenticator.CreateAndSendOtpAsync(user);
        }
    }
}
