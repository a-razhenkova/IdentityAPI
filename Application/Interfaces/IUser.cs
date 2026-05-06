namespace Application
{
    public interface IUser
    {
        Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken);

        Task<UserDto> GetAsync(string userPublicId);

        Task<string> RegisterAsync(UserDto userDto);

        Task UpdateAsync(string userPublicId, UserDto userDto);

        Task DeleteAsync(string userPublicId);

        Task ChangePasswordAsync(string userPublicId, string oldPassword, string newPassword);

        Task ChangeEmailAsync(string userPublicId, string email, string password);
    }
}