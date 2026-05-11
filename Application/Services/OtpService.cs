using Domain;
using Google.Authenticator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Application
{
    public class OtpService : IOtp
    {
        private readonly AppSettings _appSettings;
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly IRedis _redis;
        private readonly IRabbitMq _rabbitMq;

        public OtpService(IOptionsSnapshot<AppSettings> appSettings,
                         IUserAuthenticator userAuthenticator,
                         IRedis redis,
                         IRabbitMq rabbitMq)
        {
            _appSettings = appSettings.Value;
            _userAuthenticator = userAuthenticator;
            _redis = redis;
            _rabbitMq = rabbitMq;
        }

        public async Task<string> CreateAndSendAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            User user = await _userAuthenticator.AuthenticateAsync(username, password, cancellationToken);

            if (!user.IsVerified)
                throw new ForbiddenException($"User cannot receive OTP.");

            string otp = await CreateAndSaveOtp(user, cancellationToken);

            await PublishUserOtp(user, otp, cancellationToken);

            return user.PublicId;
        }

        public async Task<string> CreateAndSaveOtp(User user, CancellationToken cancellationToken = default)
        {
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            string otp = twoFactorAuthenticator.GetCurrentPIN(user.OtpKey, false);

            var otpModel = new Redis.OtpModel()
            {
                UserPublicId = user.PublicId,
                Otp = otp,
                WrongAuthAttemptsCounter = 0
            };

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_appSettings.Security.MultiFactorAuth.LifetimeInSeconds));
            await _redis.AddOrUpdateAsync(RedisKey.OneTimePassword, otpModel, cacheEntryOptions, keyIds: [user.PublicId], cancellationToken);

            return otp;
        }

        public async Task PublishUserOtp(User user, string otp, CancellationToken cancellationToken = default)
        {
            var evt = RabbitMqEventFactory.CreateNewUserOtpEvent(user, otp);
            await _rabbitMq.PublishEventAsync(evt, cancellationToken);
        }
    }
}