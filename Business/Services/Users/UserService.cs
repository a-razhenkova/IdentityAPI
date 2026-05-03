using AutoMapper;
using Business.RabbitMq;
using Database.IdentityDb;
using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.RegularExpressions;

namespace Business
{
    public class UserService : IUserHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IdentityDbContext _identityDbContext;
        private readonly IMapper _mapper;
        private readonly IReportHandler _reportHandler;
        private readonly IAlert _alert;

        public UserService(IHttpContextAccessor httpContextAccessor,
                          IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                          IdentityDbContext identityDbContext,
                          IMapper mapper,
                          IReportHandler reportHandler,
                          IAlert alert)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettingsOptions = appSettingsOptions.Value;
            _identityDbContext = identityDbContext;
            _mapper = mapper;
            _reportHandler = reportHandler;
            _alert = alert;
        }

        public async Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken)
        {
            IQueryable<User> searchQuery = _identityDbContext.User;

            if (!string.IsNullOrWhiteSpace(userSearchParams.Username))
            {
                searchQuery = searchQuery.Where(u => EF.Functions.Like(u.Username, $"%{userSearchParams.Username}%"));
            }

            if (userSearchParams.Role is not null)
            {
                searchQuery = searchQuery.Where(u => u.Role == userSearchParams.Role);
            }

            if (userSearchParams.Status is not null)
            {
                searchQuery = searchQuery.Where(u => u.Status.Value == userSearchParams.Status);
            }

            searchQuery = searchQuery
                .Include(u => u.Status)
                .OrderByDescending(u => u.Id);

            return await _reportHandler.PreparePaginatedReport<User, UserDto>(searchQuery, userSearchParams, cancellationToken);
        }

        public async Task<UserDto> LoadAsync(string userExternalId)
        {
            User user = await _identityDbContext.User.AsNoTracking()
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> RegisterAsync(UserDto userDto)
        {
            ValidatePasswordFormat(userDto.Password);

            User? user = await _identityDbContext.User.AsNoTracking()
                .Where(u => u.Username == userDto.Username)
                .SingleOrDefaultAsync();

            if (user is not null)
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");

            user = _mapper.Map<User>(userDto);
            user.ExternalId = Guid.NewGuid().ToString();
            user.Password = UserSecurePassword.Create(userDto.Password);
            user.OtpSecret = UserOtpSecret.Create();

            await _identityDbContext.AddAsync(user);
            await _identityDbContext.SaveChangesAsync();

            return user.ExternalId;
        }

        public async Task UpdateAsync(string userExternalId, UserDto userDto)
        {
            User updatedUser = await _identityDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!updatedUser.Username.Equals(userDto.Username)
                && await _identityDbContext.User.AsNoTracking().Where(u => u.Username == userDto.Username).AnyAsync())
            {
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");
            }

            User currentUser = updatedUser.DeepCopy();
            updatedUser = _mapper.Map(userDto, updatedUser);

            if (!updatedUser.IsEqual(currentUser))
            {
                await _identityDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string userExternalId)
        {
            User user = await _identityDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            // TODO: schadule for delete with Hangfire
            _identityDbContext.Remove(user);
            await _identityDbContext.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string userExternalId, string oldPassword, string newPassword)
        {
            ValidatePasswordFormat(newPassword);

            User user = await _identityDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!UserSecurePassword.IsValid(user.Password.Value, oldPassword, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Password = UserSecurePassword.Create(newPassword);

            await _identityDbContext.SaveChangesAsync();

            _alert.AddUserPasswordChangedAlertAsync(new UserPasswordChangedAlertDto
            {
                UserId = user.Id,
                UserEmail = user.Email,
                Timestamp = user.Password.LastChangedTimestamp,
                UserIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress()
            });
        }

        public async Task ChangeEmailAsync(string userExternalId, string email, string password)
        {
            ValidatePasswordFormat(password);

            User user = await _identityDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!UserSecurePassword.IsValid(user.Password.Value, password, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Email = email;
            user.IsVerified = false;
            user.Status.Value = UserStatuses.Restricted;
            user.Status.Reason = UserStatusReasons.EmailChanged;

            await _identityDbContext.SaveChangesAsync();
        }

        private void ValidatePasswordFormat(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password is required.");

            if (!new Regex(_appSettingsOptions.Security.PasswordValidationRegex).Match(password).Success)
                throw new BadRequestException($"Password must match the regular expression '{_appSettingsOptions.Security.PasswordValidationRegex}'.");
        }
    }
}