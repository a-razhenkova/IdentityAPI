using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IdentityDb
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IdentityContext context) : base(context) { }

        public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(id, autoTrack: true).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByIdWithNoTrackingAsync(string id, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(id, autoTrack: false).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(username, autoTrack: true).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByUsernameWithNoTrackingAsync(string username, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(username, autoTrack: false).SingleOrDefaultAsync(cancellationToken);

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            user.PublicId = Guid.NewGuid().ToString();
            user.Password = UserPasswordHandler.Create(password);
            user.OtpSecret = UserOtpHandler.Create();

            await _context.AddAsync(user, cancellationToken);
        }

        public IQueryable<User> WhereIdEquals(string id, bool autoTrack = true)
            => Where(x => x.PublicId.Equals(id), autoTrack: autoTrack);

        public IQueryable<User> WhereUsernameEquals(string username, bool autoTrack = true)
            => Where(x => x.Username.Equals(username), autoTrack: autoTrack);
    }
}