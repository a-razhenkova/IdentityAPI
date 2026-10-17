using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application
{
    public class UserAuthenticationService : IUserAuthenticator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMq _rebbitMq;

        public UserAuthenticationService(
            IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<AppSettings> appSettings,
            IUnitOfWork unitOfWork,
            IRabbitMq rebbitMq)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _rebbitMq = rebbitMq;
        }

        public async Task<User> AuthenticateAsync(string userReference, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Reference.Equals(userReference))
                .Include(u => u.Status)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException();

            if (!user.IsActivate())
                throw new ForbiddenException(AppMessages.UserStatusIs(user.Status.Value));

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Username.Equals(username))
                .Include(u => u.Status)
                .Include(u => u.Password)
                .Include(u => u.Login)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException("Invalid credentials.");

            if (!user.IsActivate())
                throw new ForbiddenException(AppMessages.UserStatusIs(user.Status.Value));

            user.Login ??= new Login();

            if (!user.Password.IsMatch(password))
            {
                await ProcessWrongLoginAttempt(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException(AppMessages.InvalidCredentials);
            }

            await ProcessSuccessfulLoginAttempt(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user;
        }

        private async Task ProcessSuccessfulLoginAttempt(User user)
        {
            user.Login.ResetCounter();

            string? userIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();
            if (!string.IsNullOrWhiteSpace(userIpAddress) && !userIpAddress.Equals(user.Login.LastLoginIpAddress))
            {
                user.Login.LastLoginIpAddress = userIpAddress;

                var evt = RabbitMqEventFactory.CreateLoginFromNewIpAddressEvent(user);
                await _rebbitMq.PublishEventInBackgroundAsync(evt); // cancellation is unnecessary because login is already processed
            }
        }

        private async Task ProcessWrongLoginAttempt(User user)
        {
            user.Login.WrongLoginAttemptsCounter++;

            if (user.Login.WrongLoginAttemptsCounter >= _appSettings.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
            {
                user.Block(UserStatusReasons.MaxWrongLoginAttemptsReached);

                var evt = RabbitMqEventFactory.CreateLoginAttemptMadeEvent(user);
                await _rebbitMq.PublishEventInBackgroundAsync(evt); // cancellation is unnecessary because login is already processed
            }
        }
    }
}