using Application.Redis;
using Domain;
using Google.Authenticator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shared;

namespace Application
{
    public class OtpAuthenticationService : IOtpAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedis _redis;

        public OtpAuthenticationService(IOptionsSnapshot<AppSettings> appSettings,
                                       IUnitOfWork unitOfWork,
                                       IRedis redis)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<string> CreateAndSendOtpAsync(User user)
        {
            CheckUserStatus(user.Status);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            string otp = twoFactorAuthenticator.GetCurrentPIN(user.OtpSecret, false);

            var otpModel = new OtpModel()
            {
                UserPublicId = user.PublicId,
                Otp = otp,
                WrongAuthAttemptsCounter = 0
            };

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_appSettings.Security.MultiFactorAuth.LifetimeInSeconds));
            await _redis.AddOrUpdateAsync(RedisKey.OneTimePassword, otpModel, cacheEntryOptions, keyIds: user.PublicId);

            // TODO: send OTP to user via email

            return user.PublicId;
        }

        public async Task<User> ValidateOtpAsync(string userPublicId, string otp)
        {
            OtpModel otpModel = await _redis.LoadAsync<OtpModel>(RedisKey.OneTimePassword, userPublicId)
                ?? throw new UnauthorizedException("The code has expired.");

            if (!otpModel.Otp.Equals(otp))
            {
                await ProcessLoginAttemptAsync(otpModel, isLoginSuccessful: false);
                throw new UnauthorizedException("Invalid code.");
            }

            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPublicId, loadStatus: true, loadLogin: true)
               ?? throw new UnauthorizedException("The code has expired.");

            CheckUserStatus(user.Status);

            bool isPinValid = new TwoFactorAuthenticator().ValidateTwoFactorPIN(user.OtpSecret, otp, true);

            await ProcessLoginAttemptAsync(otpModel, isPinValid);

            if (!isPinValid)
                throw new UnauthorizedException("Invalid code.");

            return user;
        }

        private void CheckUserStatus(UserStatus status)
        {
            if (status.Value == UserStatuses.Restricted
             && status.Value == UserStatuses.Blocked
             && status.Value == UserStatuses.Disabled)
            {
                throw new ForbiddenException($"User status is '{status.Value}'.");
            }
        }

        private async Task ProcessLoginAttemptAsync(OtpModel otpModel, bool isLoginSuccessful)
        {
            if (isLoginSuccessful)
            {
                await _redis.DeleteAsync(RedisKey.OneTimePassword, otpModel.UserPublicId);
            }
            else
            {
                otpModel.WrongAuthAttemptsCounter++;

                if (otpModel.WrongAuthAttemptsCounter >= _appSettings.Security.MultiFactorAuth.DefaultMaxWrongLoginAttemptsBeforeBlock)
                {
                    await _redis.DeleteAsync(RedisKey.OneTimePassword, otpModel.UserPublicId);
                }
                else
                {
                    await _redis.UpdateAsync(RedisKey.OneTimePassword, otpModel, otpModel.UserPublicId);
                }
            }
        }
    }
}