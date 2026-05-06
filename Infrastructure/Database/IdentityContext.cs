using Domain;
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

        public DbSet<Login> Login { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Subscription> Subscription { get; set; }

        public DbSet<Client> Client { get; set; }
        public DbSet<ClientStatus> ClientStatus { get; set; }
        public DbSet<ClientRight> ClientRight { get; set; }
        public DbSet<ClientSubscription> ClientSubscription { get; set; }

        public DbSet<User> User { get; set; }
        public DbSet<UserPassword> UserPassword { get; set; }
        public DbSet<UserStatus> UserStatus { get; set; }
    }
}