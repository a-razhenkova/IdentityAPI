using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.IdentityDb
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IdentityContext context) : base(context)
        {

        }

        public async Task<User?> GetByPublicIdAsync(string id, bool autoTrack = false, bool loadStatus = false, bool loadPassword = false, bool loadLogin = false)
            => await GetQueryAsync(u => u.PublicId == id, autoTrack, loadStatus, loadPassword, loadLogin).SingleOrDefaultAsync();

        public async Task<User?> GetByUsernameAsync(string username, bool autoTrack = false, bool loadStatus = false, bool loadPassword = false, bool loadLogin = false)
            => await GetQueryAsync(u => u.Username == username, autoTrack, loadStatus, loadPassword, loadLogin).SingleOrDefaultAsync();

        public IQueryable<User> GetQueryAsync(Expression<Func<User, bool>> expression,
            bool autoTrack = false,
            bool loadStatus = false,
            bool loadPassword = false,
            bool loadLogin = false)
        {
            IQueryable<User> query = base.GetQueryAsync(expression, autoTrack: autoTrack);

            if (loadStatus)
                query = query.Include(u => u.Status);

            if (loadPassword)
                query = query.Include(u => u.Password);

            if (loadLogin)
                query = query.Include(u => u.Login);

            return query;
        }

        public async Task AddAsync(User user, string password)
        {
            user.PublicId = Guid.NewGuid().ToString();
            user.Password = UserSecurePassword.Create(password);
            user.OtpSecret = UserOtpSecret.Create();

            await _context.AddAsync(user);
        }
    }
}