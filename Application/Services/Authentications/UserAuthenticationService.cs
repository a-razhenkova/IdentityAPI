using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared;

namespace Application
{
    public class UserAuthenticationService : IUserAuthenticator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRabbitMq _rebbitMq;

        public UserAuthenticationService(IHttpContextAccessor httpContextAccessor,
                                        IOptionsSnapshot<AppSettings> appSettings,
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        IRabbitMq rebbitMq)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rebbitMq = rebbitMq;
        }

        public async Task<User> AuthenticateAsync(string userPublicId)
        {
            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPublicId, autoTrack: true, loadStatus: true, loadLogin: true)
               ?? throw new UnauthorizedException();

            CheckUserStatus(user.Status);

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            User user = await _unitOfWork.Users.GetByUsernameAsync(username, autoTrack: true, loadStatus: true, loadPassword: true, loadLogin: true)
                ?? throw new UnauthorizedException("Invalid credentials.");

            CheckUserStatus(user.Status);

            bool isPasswordValid = UserSecurePassword.IsValid(user.Password.Value, password, user.Password.Secret);

            user.Login ??= new Login();
            string? lastLoginIpAddress = user.Login.LastLoginIpAddress;

            await ProcessLoginAttempt(user, isPasswordValid);

            if (user.Status.Value == UserStatuses.Blocked)
            {
                var evt = _mapper.Map<RabbitMq.LoginAttemptMadeEvent>(user);
                await _rebbitMq.PublishEventAsync(evt);
            }

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid credentials.");

            if (lastLoginIpAddress is not null && lastLoginIpAddress.Equals(user.Login.LastLoginIpAddress))
            {
                var evt = _mapper.Map<RabbitMq.LoginFromNewIpAddressEvent>(user);
                await _rebbitMq.PublishEventAsync(evt);
            }

            return user;
        }

        private void CheckUserStatus(UserStatus status)
        {
            if (status.Value == UserStatuses.Blocked
             && status.Value == UserStatuses.Disabled)
            {
                throw new ForbiddenException($"User status is '{status.Value}'.");
            }
        }

        private async Task ProcessLoginAttempt(User user, bool isPasswordValid)
        {
            if (isPasswordValid)
            {
                user.Login.WrongLoginAttemptsCounter = 0;
                user.Login.LastLoginDate = DateTime.UtcNow;

                string? userIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();
                if (!string.IsNullOrWhiteSpace(userIpAddress))
                    user.Login.LastLoginIpAddress = userIpAddress;
            }
            else
            {
                user.Login.WrongLoginAttemptsCounter++;

                if (user.Login.WrongLoginAttemptsCounter >= _appSettings.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                    user.Block(UserStatusReasons.MaxWrongLoginAttemptsReached);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}