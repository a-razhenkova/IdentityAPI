using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using System.Data;
using System.Text.RegularExpressions;

namespace Application
{
    public class UserService : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedReport _paginatedReport;
        private readonly IRabbitMq _rebbitMq;

        public UserService(IHttpContextAccessor httpContextAccessor,
                          IOptionsSnapshot<AppSettings> appSettings,
                          IUnitOfWork unitOfWork,
                          IMapper mapper,
                          IPaginatedReport paginatedReport,
                          IRabbitMq rabbitMq)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginatedReport = paginatedReport;
            _rebbitMq = rabbitMq;
        }

        public async Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken)
        {
            IQueryable<User> searchQuery = _unitOfWork.Users.GetRepo();

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

            return await _paginatedReport.Prepare<User, UserDto>(searchQuery, userSearchParams, cancellationToken);
        }

        public async Task<UserDto> GetAsync(string userPubliclId)
        {
            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPubliclId, loadStatus: true)
                ?? throw new NotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> RegisterAsync(UserDto userDto)
        {
            ValidatePasswordFormat(userDto.Password);

            User? user = await _unitOfWork.Users.GetAsync(u => u.Username == userDto.Username);

            if (user is not null)
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");

            user = _mapper.Map<User>(userDto);

            await _unitOfWork.Users.AddAsync(user, userDto.Password);
            await _unitOfWork.SaveChangesAsync();

            return user.PublicId;
        }

        public async Task UpdateAsync(string userPublicId, UserDto userDto)
        {
            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPublicId, autoTrack: true, loadStatus: true, loadPassword: true)
                ?? throw new NotFoundException("User not found.");

            if (!user.Username.Equals(userDto.Username))
            {
                if (await _unitOfWork.Users.ExistAsync(u => u.Username == userDto.Username))
                    throw new ConflictException($"User with username '{userDto.Username}' already registered.");
            }

            User userSnapshot = user.DeepCopy();
            user = _mapper.Map(userDto, user);

            bool hasChanges = !user.IsEqual(userSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges);
        }

        public async Task DeleteAsync(string userPublicId)
        {
            User? user = await _unitOfWork.Users.GetAsync(u => u.PublicId == userPublicId)
                ?? throw new NotFoundException("User not found.");

            // TODO: schadule for delete with Hangfire
            _unitOfWork.Users.BasicRemove(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string userPublicId, string oldPassword, string newPassword)
        {
            ValidatePasswordFormat(newPassword);

            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPublicId, autoTrack: true, loadPassword: true)
                ?? throw new NotFoundException("User not found.");

            if (!UserSecurePassword.IsValid(user.Password.Value, oldPassword, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Password = UserSecurePassword.Create(newPassword);

            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var message = _mapper.Map<RabbitMq.UserPasswordChangedEvent>(user);
                message.UserIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();

                _rebbitMq.PublishEventAsync(message);
            }
        }

        public async Task ChangeEmailAsync(string userPublicId, string email, string password)
        {
            ValidatePasswordFormat(password);

            User user = await _unitOfWork.Users.GetByPublicIdAsync(userPublicId, autoTrack: true, loadStatus: true, loadPassword: true)
                ?? throw new NotFoundException("User not found.");

            // TODO: try improving
            if (!UserSecurePassword.IsValid(user.Password.Value, password, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Email = email;
            user.IsVerified = false;
            user.Status.Value = UserStatuses.Restricted;
            user.Status.Reason = UserStatusReasons.EmailChanged;

            await _unitOfWork.SaveChangesAsync();
        }

        private void ValidatePasswordFormat(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password is required.");

            if (!new Regex(_appSettings.Security.PasswordValidationRegex).IsMatch(password))
                throw new BadRequestException($"Password must match the regular expression '{_appSettings.Security.PasswordValidationRegex}'.");
        }
    }
}