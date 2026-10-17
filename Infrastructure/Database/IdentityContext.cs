using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure
{
    [Database(ConnectionStringNames.IdentityDb)]
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) 
            : base(options)
        {

        }
    }
}