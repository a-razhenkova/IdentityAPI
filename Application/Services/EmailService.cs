using Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace Application
{
    public interface IEmail
    {
        Task VerifyToken(string token, CancellationToken cancellationToken = default);
    }

    public class EmailService : IEmail
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IOptionsSnapshot<AppSettings> appSettings,
                           IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task VerifyToken(string token, CancellationToken cancellationToken = default)
        {
            var emailToken = new EmailVerificationToken(token, _appSettings.Security);

            TokenValidationResult validationResult = await emailToken.ValidateAsync();

            if (!validationResult.IsValid)
                throw new UnprocessableContentException("Invalid verification code.");

            string userPublicId = emailToken.GetClaim(TokenClaim.UserPublicId)
                ?? throw new UnauthorizedException("Invalid token.");

            await UpdateUserStatus(userPublicId, cancellationToken);
        }

        private async Task UpdateUserStatus(string userPublicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users.GetByIdAsync(userPublicId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            if (user.IsVerified)
                return;

            user.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}