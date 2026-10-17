using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application
{
    public class UserService : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginatedReport _paginatedReport;
        private readonly IRabbitMq _rebbitMq;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
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
            IQueryable<User> userQuery = _unitOfWork.Users
                .AsNoTracking()
                .ToQuery();

            if (!string.IsNullOrWhiteSpace(query.Username))
                userQuery = userQuery.Where(u => EF.Functions.Like(u.Username, $"%{query.Username}%"));

            if (query.Role != null)
                userQuery = userQuery.Where(u => u.Role == query.Role);

            if (query.Status != null)
                userQuery = userQuery.Where(u => u.Status.Value == query.Status);

            userQuery = userQuery
                .Include(u => u.Status)
                .OrderByDescending(u => u.Id);

            return await _paginatedReport.Prepare(userQuery, query, UserMapper.MapToDto, cancellationToken);
        }

        public async Task<UserDto> GetAsync(string userReference, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Reference.Equals(userReference))
                .Include(u => u.Status)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.UserNotFound);

            return UserMapper.MapToDto(user);
        }

        public async Task<string> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = PasswordValidator.Validate(command.Password, _appSettings);
            if (passwordValidationError != null)
                throw new BadRequestException(passwordValidationError);

            User? user = await _unitOfWork.Users
                .Where(u => u.Username == command.Username)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (user != null)
                throw new ConflictException(AppMessages.UserAlreadyExists(command.Username));

            user = new User().Map(command);

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                string emailToken = new EmailVerificationToken(_appSettings.Security).Create(user);

                var evt = RabbitMqEventFactory.CreateEmailVerificationEvent(user, emailToken);
                _ = _rebbitMq.PublishEventAsync(evt); // cancellation is unnecessary because changes are already made
            }

            return user.Reference;
        }

        public async Task UpdateAsync(string userReference, UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Reference == userReference)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.UserNotFound);

            if (!user.Username.Equals(command.Username))
            {
                bool isUsernameTaken = await _unitOfWork.Users.AnyAsync(u => u.Username == command.Username, cancellationToken);

                if (isUsernameTaken)
                    throw new ConflictException(AppMessages.UserAlreadyExists(command.Username));
            }

            User userSnapshot = user.DeepCopy();
            user.Map(command);

            bool hasChanges = !user.IsEqual(userSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges, cancellationToken);
        }

        public async Task DeleteAsync(string userReference, CancellationToken cancellationToken = default)
        {
            User? user = await _unitOfWork.Users
                .Where(u => u.Reference == userReference)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.UserNotFound);

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdatePasswordAsync(string userReference, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = PasswordValidator.Validate(newPassword, _appSettings);
            if (passwordValidationError != null)
                throw new BadRequestException(passwordValidationError);

            User user = await _unitOfWork.Users
                .Where(u => u.Reference == userReference)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException(AppMessages.UserNotFound);

            if (!user.Password.IsMatch(oldPassword))
                throw new BadRequestException(AppMessages.InvalidPassword);

            user.Password = UserPassword.Create(newPassword);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var evt = RabbitMqEventFactory.CreateUserPasswordChangedEvent(user, _httpContextAccessor?.HttpContext?.GetUserIpAddress()); 
                _ = _rebbitMq.PublishEventAsync(evt); // cancellation is unnecessary because changes are already made
            }
        }

        public async Task UpdateEmailAsync(string userReference, string email, string password, CancellationToken cancellationToken = default)
        {
            string? passwordValidationError = PasswordValidator.Validate(password, _appSettings);
            if (passwordValidationError != null)
                throw new BadRequestException(passwordValidationError);

            User user = await _unitOfWork.Users
                .Where(u => u.Reference == userReference)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.UserNotFound);

            if (!user.Password.IsMatch(password))
                throw new BadRequestException(AppMessages.InvalidPassword);

            user.Email = email;
            user.Restrict(UserStatusReasons.EmailChanged);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _ = SendEmailVerification(user); // cancellation is unnecessary because changes are already made
        }

        public async Task CreateAndSendEmailVerificationAsync(string userReference, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .Where(u => u.Reference == userReference)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(AppMessages.UserNotFound);

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ForbiddenException(AppMessages.UserEmailNotFound);

            await SendEmailVerification(user);
        }

        private async Task SendEmailVerification(User user, CancellationToken cancellationToken = default)
        {
            string emailToken = new EmailVerificationToken(_appSettings.Security).Create(user);

            var evt = RabbitMqEventFactory.CreateEmailVerificationEvent(user, emailToken);
            await _rebbitMq.PublishEventAsync(evt, cancellationToken);
        }
    }
}