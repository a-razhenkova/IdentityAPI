using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using System.Data;

namespace Application
{
    public class UserService : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginatedReport _paginatedReport;
        private readonly IRabbitMq _rebbitMq;

        public UserService(IHttpContextAccessor httpContextAccessor,
                          IOptionsSnapshot<AppSettings> appSettings,
                          IUnitOfWork unitOfWork,
                          IPaginatedReport paginatedReport,
                          IRabbitMq rabbitMq)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _paginatedReport = paginatedReport;
            _rebbitMq = rabbitMq;
        }

        public async Task<PaginatedReportDto<UserDto>> SearchAsync(SearchUserQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<User> userQuery = _unitOfWork.Users.Init().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Username))
                userQuery = userQuery.Where(u => EF.Functions.Like(u.Username, $"%{query.Username}%"));

            if (query.Role is not null)
                userQuery = userQuery.Where(u => u.Role == query.Role);

            if (query.Status is not null)
                userQuery = userQuery.Where(u => u.Status.Value == query.Status);

            userQuery = userQuery
                .Include(u => u.Status)
                .OrderByDescending(u => u.Id);

            return await _paginatedReport.Prepare(userQuery, query, UserMapper.MapToDto, cancellationToken);
        }

        public async Task<UserDto> GetAsync(string userPublicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users.GetByIdWithNoTrackingAsync(userPublicId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            return UserMapper.MapToDto(user);
        }

        public async Task<string> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = _appSettings.Security.ValidatePassword(command.Password);
            if (passwordValidationError is not null)
                throw new BadRequestException(passwordValidationError);

            User? user = await _unitOfWork.Users.GetByUsernameAsync(command.Username, cancellationToken);

            if (user is not null)
                throw new ConflictException($"User with username '{command.Username}' already registered.");

            user = new User().Map(command);

            await _unitOfWork.Users.BasicAddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                string emailToken = new EmailVerificationToken(_appSettings.Security).Create(user);

                var evt = RabbitMqEventFactory.CreateEmailVerificationEvent(emailToken);
                await _rebbitMq.PublishEventInBackground(evt); // cancellation is unnecessary because changes are already made
            }

            return user.PublicId;
        }

        public async Task UpdateAsync(string userPublicId, UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("User not found.");

            if (!user.Username.Equals(command.Username))
            {
                bool isUsernameTaken = await _unitOfWork.Users.CheckIfExistAsync(expression: u => u.Username == command.Username);

                if (isUsernameTaken)
                    throw new ConflictException($"User with username '{command.Username}' already registered.");
            }

            User userSnapshot = user.DeepCopy();
            user.Map(command);

            bool hasChanges = !user.IsEqual(userSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges, cancellationToken);
        }

        public async Task DeleteAsync(string userPublicId, CancellationToken cancellationToken = default)
        {
            User? user = await _unitOfWork.Users.GetByIdAsync(userPublicId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            _unitOfWork.Users.BasicRemove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdatePasswordAsync(string userPublicId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = _appSettings.Security.ValidatePassword(newPassword);
            if (passwordValidationError is not null)
                throw new BadRequestException(passwordValidationError);

            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!user.Password.IsMatch(oldPassword))
                throw new BadRequestException("Invalid old password.");

            user.Password = UserPassword.Create(newPassword);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var evt = RabbitMqEventFactory.CreateUserPasswordChangedEvent(user, _httpContextAccessor?.HttpContext?.GetUserIpAddress()); 
                await _rebbitMq.PublishEventInBackground(evt); // cancellation is unnecessary because changes are already made
            }
        }

        public async Task UpdateEmailAsync(string userPublicId, string email, string password, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = _appSettings.Security.ValidatePassword(password);
            if (passwordValidationError is not null)
                throw new BadRequestException(passwordValidationError);

            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("User not found.");

            if (!user.Password.IsMatch(password))
                throw new BadRequestException("Invalid old password.");

            user.Email = email;
            user.Restrict(UserStatusReasons.EmailChanged);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // sending email verification
            string emailToken = new EmailVerificationToken(_appSettings.Security).Create(user);

            var evt = RabbitMqEventFactory.CreateEmailVerificationEvent(emailToken);
            await _rebbitMq.PublishEventInBackground(evt); // cancellation is unnecessary because changes are already made
        }

        public async Task CreateAndSendEmailVerificationAsync(string userPublicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users.GetByIdWithNoTrackingAsync(userPublicId)
                ?? throw new NotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ForbiddenException("User email is missing.");

            string emailToken = new EmailVerificationToken(_appSettings.Security).Create(user);

            var evt = RabbitMqEventFactory.CreateEmailVerificationEvent(emailToken);
            await _rebbitMq.PublishEventAsync(evt, cancellationToken);
        }
    }
}