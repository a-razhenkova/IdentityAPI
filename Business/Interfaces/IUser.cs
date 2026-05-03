namespace Business
{
    public interface IUser
    {
        Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken);

        Task<UserDto> LoadAsync(string userExternalId);

        Task<string> RegisterAsync(UserDto userDto);

        Task UpdateAsync(string userExternalId, UserDto userDto);

        Task DeleteAsync(string userExternalId);

        Task ChangePasswordAsync(string userExternalId, string oldPassword, string newPassword);

        Task ChangeEmailAsync(string userExternalId, string email, string password);
    }
}