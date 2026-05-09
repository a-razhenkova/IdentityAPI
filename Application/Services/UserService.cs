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

        public async Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken = default)
        {
            IQueryable<User> searchQuery = _unitOfWork.Users.Init().AsNoTracking();

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

        public async Task<UserDto> GetAsync(string userPublicId, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId, autoTrack: false)
                .Include(u => u.Status)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default)
        {
            UserPasswordValidator.Validate(userDto.Password, _appSettings.Security);

            User? user = await _unitOfWork.Users.GetByUsernameAsync(userDto.Username, cancellationToken);

            if (user is not null)
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");

            user = _mapper.Map<User>(userDto);

            await _unitOfWork.Users.AddAsync(user, userDto.Password);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.PublicId;
        }

        public async Task UpdateAsync(string userPublicId, UserDto userDto, CancellationToken cancellationToken = default)
        {
            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("User not found.");

            if (!user.Username.Equals(userDto.Username))
            {
                bool isUsernameTaken = await _unitOfWork.Users.CheckIfExistAsync(expression: u => u.Username == userDto.Username);

                if (!isUsernameTaken)
                    throw new ConflictException($"User with username '{userDto.Username}' already registered.");
            }

            User userSnapshot = user.DeepCopy();
            user = _mapper.Map(userDto, user);

            bool hasChanges = !user.IsEqual(userSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges, cancellationToken);
        }

        public async Task DeleteAsync(string userPublicId, CancellationToken cancellationToken = default)
        {
            User? user = await _unitOfWork.Users.GetByIdAsync(userPublicId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            // TODO: schadule for delete with Hangfire
            _unitOfWork.Users.BasicRemove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(string userPublicId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            UserPasswordValidator.Validate(newPassword, _appSettings.Security);

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
                var evt = _mapper.Map<RabbitMq.UserPasswordChangedEvent>(user);
                evt.UserIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();

                // cancellation is unnecessary because changes are already made
                await _rebbitMq.PublishEventInBackground(evt);
            }
        }

        public async Task ChangeEmailAsync(string userPublicId, string email, string password, CancellationToken cancellationToken = default)
        {
            UserPasswordValidator.Validate(password, _appSettings.Security);

            User user = await _unitOfWork.Users
                .WhereIdEquals(userPublicId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("User not found.");

            if (!user.Password.IsMatch(password))
                throw new BadRequestException("Invalid old password.");

            user.Email = email;
            user.IsVerified = false;
            user.Status.Value = UserStatuses.Restricted;
            user.Status.Reason = UserStatusReasons.EmailChanged;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}