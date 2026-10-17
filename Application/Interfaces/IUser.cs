namespace Application
{
    public interface IUser
    {
        Task<PaginatedReportDto<UserDto>> SearchAsync(SearchUserQuery query, CancellationToken cancellationToken = default);

        Task<UserDto> GetAsync(string userReference, CancellationToken cancellationToken = default);

        Task<string> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default);

        Task UpdateAsync(string userReference, UpdateUserCommand command, CancellationToken cancellationToken = default);

        Task DeleteAsync(string userReference, CancellationToken cancellationToken = default);

        Task UpdatePasswordAsync(string userReference, string oldPassword, string newPassword, CancellationToken cancellationToken = default);

        Task UpdateEmailAsync(string userReference, string email, string password, CancellationToken cancellationToken = default);

        Task CreateAndSendEmailVerificationAsync(string userReference, CancellationToken cancellationToken = default);
    }
}