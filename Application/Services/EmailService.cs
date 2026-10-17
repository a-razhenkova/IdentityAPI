using Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace Application
{
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
                throw new UnprocessableContentException(AppMessages.InvalidVerificationCode);

            string userPublicId = emailToken.GetClaim(TokenClaim.UserReference)
                ?? throw new UnauthorizedException(AppMessages.InvalidToken);

            await UpdateUserStatus(userPublicId, cancellationToken);
        }

        private async Task UpdateUserStatus(string userPublicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Reference.Equals(userPublicId))
                .Include(u => u.Status)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.UserNotFound);

            if (user.IsVerified)
                return;

            user.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}