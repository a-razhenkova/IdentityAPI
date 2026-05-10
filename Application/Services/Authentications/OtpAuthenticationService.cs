using Application.Redis;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

        public async Task<User> AuthenticateAsync(string userPublicId, string otp, CancellationToken cancellationToken = default)
        {
            OtpModel otpModel = await _redis.LoadAsync<OtpModel>(RedisKey.OneTimePassword, [userPublicId], cancellationToken)
                ?? throw new UnauthorizedException("The code has expired.");

            if (!otpModel.Otp.Equals(otp))
            {
                await ProcessWrongLoginAttempt(otpModel, cancellationToken);
                throw new UnauthorizedException("Invalid code.");
            }

            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Login)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException("The code has expired.");

            if (!user.IsActivate())
                throw new ForbiddenException($"User status is '{user.Status.Value}'.");

            await _redis.DeleteAsync(RedisKey.OneTimePassword, [otpModel.UserPublicId], cancellationToken);

            return user;
        }

        private async Task ProcessWrongLoginAttempt(OtpModel otp, CancellationToken cancellationToken = default)
        {
            otp.WrongAuthAttemptsCounter++;

            if (otp.WrongAuthAttemptsCounter >= _appSettings.Security.MultiFactorAuth.DefaultMaxWrongLoginAttemptsBeforeBlock)
            {
                await _redis.DeleteAsync(RedisKey.OneTimePassword, keyIds: [otp.UserPublicId], cancellationToken);
            }
            else
            {
                await _redis.UpdateAsync(RedisKey.OneTimePassword, otp, keyIds: [otp.UserPublicId], cancellationToken);
            }
        }
    }
}