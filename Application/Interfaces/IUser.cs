namespace Application
{
    public interface IUser
    {
        Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken = default);

        Task<UserDto> GetAsync(string userPublicId, CancellationToken cancellationToken = default);

        Task<string> RegisterAsync(UserDto userDto, CancellationToken cancellationToken = default);

        Task UpdateAsync(string userPublicId, UserDto userDto, CancellationToken cancellationToken = default);

        Task DeleteAsync(string userPublicId, CancellationToken cancellationToken = default);

        Task ChangePasswordAsync(string userPublicId, string oldPassword, string newPassword, CancellationToken cancellationToken = default);

        Task ChangeEmailAsync(string userPublicId, string email, string password, CancellationToken cancellationToken = default);

        Task SendEmailVerificationAsync(string userPublicId, CancellationToken cancellationToken = default);
    }
}