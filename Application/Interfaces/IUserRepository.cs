using Domain;

namespace Application
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithNoTrackingAsync(string id, CancellationToken cancellationToken = default);

        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameWithNoTrackingAsync(string username, CancellationToken cancellationToken = default);

        Task AddAsync(User user, string password, CancellationToken cancellationToken = default);

        IQueryable<User> WhereIdEquals(string id, bool autoTrack = true);

        IQueryable<User> WhereUsernameEquals(string username, bool autoTrack = true);
    }
}