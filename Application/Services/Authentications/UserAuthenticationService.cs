using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User> AuthenticateAsync(string publicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users.GetByIdWithNoTrackingAsync(publicId, cancellationToken)
                ?? throw new UnauthorizedException();

            if (!user.IsActivate())
                throw new ForbiddenException($"User status is '{user.Status.Value}'.");

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .WhereUsernameEquals(username)
                .Include(u => u.Password)
                .Include(u => u.Login)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException("Invalid credentials.");

            if (!user.IsActivate())
                throw new ForbiddenException($"User status is '{user.Status.Value}'.");

            user.Login ??= new Login();

            if (!user.Password.IsMatch(password))
            {
                await ProcessWrongLoginAttempt(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("Invalid credentials.");
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

                var evt = _mapper.Map<RabbitMq.LoginFromNewIpAddressEvent>(user);
                await _rebbitMq.PublishEventInBackground(evt); // cancellation is unnecessary because login is already processed
            }
        }

        private async Task ProcessWrongLoginAttempt(User user)
        {
            user.Login.WrongLoginAttemptsCounter++;

            if (user.Login.WrongLoginAttemptsCounter >= _appSettings.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
            {
                user.Block(UserStatusReasons.MaxWrongLoginAttemptsReached);

                var evt = _mapper.Map<RabbitMq.LoginAttemptMadeEvent>(user);
                await _rebbitMq.PublishEventInBackground(evt); // cancellation is unnecessary because login is already processed
            }
        }
    }
}