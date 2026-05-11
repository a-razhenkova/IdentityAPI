using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.IdentityDb
{
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IdentityContext context) : base(context) { }

        public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => await WhereIdEquals(id, autoTrack: true).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByIdWithNoTrackingAsync(string id, CancellationToken cancellationToken = default)
            => await WhereIdEquals(id, autoTrack: false).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(username, autoTrack: true).SingleOrDefaultAsync(cancellationToken);

        public async Task<User?> GetByUsernameWithNoTrackingAsync(string username, CancellationToken cancellationToken = default)
            => await WhereUsernameEquals(username, autoTrack: false).SingleOrDefaultAsync(cancellationToken);

        public IQueryable<User> WhereIdEquals(string id, bool autoTrack = true)
            => Where(u => u.PublicId.Equals(id), autoTrack: autoTrack);

        public IQueryable<User> WhereUsernameEquals(string username, bool autoTrack = true)
            => Where(u => u.Username.Equals(username), autoTrack: autoTrack);

        public override IQueryable<User> Where(Expression<Func<User, bool>> expression, bool autoTrack = true)
            => base.Where(expression, autoTrack).Include(u => u.Status);
    }
}