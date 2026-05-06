using Domain;
using System.Linq.Expressions;

namespace Application
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByPublicIdAsync(string id, bool autoTrack = false, bool loadStatus = false, bool loadPassword = false, bool loadLogin = false);

        Task<User?> GetByUsernameAsync(string username, bool autoTrack = false, bool loadStatus = false, bool loadPassword = false, bool loadLogin = false);

        IQueryable<User> GetQueryAsync(Expression<Func<User, bool>> expression,
            bool autoTrack = false,
            bool loadStatus = false,
            bool loadPassword = false,
            bool loadLogin = false);

        Task AddAsync(User user, string password);
    }
}