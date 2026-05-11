namespace Application
{
    public interface IUser
    {
        Task<PaginatedReportDto<UserDto>> SearchAsync(SearchUserQuery query, CancellationToken cancellationToken = default);

        Task<UserDto> GetAsync(string userPublicId, CancellationToken cancellationToken = default);

        Task<string> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default);

        Task UpdateAsync(string userPublicId, UpdateUserCommand command, CancellationToken cancellationToken = default);

        Task DeleteAsync(string userPublicId, CancellationToken cancellationToken = default);

        Task UpdatePasswordAsync(string userPublicId, string oldPassword, string newPassword, CancellationToken cancellationToken = default);

        Task UpdateEmailAsync(string userPublicId, string email, string password, CancellationToken cancellationToken = default);

        Task CreateAndSendEmailVerificationAsync(string userPublicId, CancellationToken cancellationToken = default);
    }
}