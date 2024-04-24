using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Repositories.EFCore
{
    public class RepositoryContext(DbContextOptions options) : IdentityDbContext<BaseUser>(options)
    {

        public DbSet<Users> Users { get; set; }
        public DbSet<Follows> Follows { get; set; }
        public DbSet<BlockedUsers> BlockedUsers { get; set; }
        public DbSet<Tweets> Tweets { get; set; }
        public DbSet<TweetMedias> TweetMedias { get; set; }

        public DbSet<TweetLikes> TweetLikes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
