using Application.Redis;
using Domain;
using Microsoft.Extensions.Options;

namespace Application
{
    public class OtpAuthenticationService : IOtpAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedis _redis;

        public OtpAuthenticationService(
            IOptionsSnapshot<AppSettings> appSettings,
            IUnitOfWork unitOfWork,
            IRedis redis)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<User> AuthenticateAsync(string userReference, string otp, CancellationToken cancellationToken = default)
        {
            OtpModel otpModel = await _redis.LoadAsync<OtpModel>
            (
                keyType: RedisKey.OneTimePassword,
                keyIds:
                [
                    userReference
                ],
                cancellationToken: cancellationToken
            ) ?? throw new UnauthorizedException(AppMessages.ExpiredCode);

            if (!otpModel.Otp.Equals(otp))
            {
                await ProcessWrongLoginAttempt(otpModel, cancellationToken);
                throw new UnauthorizedException(AppMessages.InvalidCode);
            }

            User user = await _unitOfWork.Users
                .Where(u => u.Reference.Equals(userReference))
                .Include(u => u.Status)
                .Include(u => u.Login)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException(AppMessages.ExpiredCode);

            if (!user.IsActivate())
                throw new ForbiddenException(AppMessages.UserStatusIs(user.Status.Value));

            await _redis.DeleteAsync
            (
                keyType: RedisKey.OneTimePassword,
                keyIds:
                [
                    otpModel.UserPublicId
                ],
                cancellationToken: cancellationToken
            );

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