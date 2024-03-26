
using Microsoft.EntityFrameworkCore;
using Repositories.EFCore;

namespace XWebAPI.Tests.Fixtures
{
    public class FollowsFixtures
    {


        public static async Task<RepositoryContext> GetDatabaseContextWithFollowers(string userId, int count = 10)
        {
            var options = new DbContextOptionsBuilder<RepositoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RepositoryContext(options);
            databaseContext.Database.EnsureCreated();
            if (!await databaseContext.Follows.AnyAsync())
            {
                for (int i = 1; i <= count; i++)
                {
                    databaseContext.Follows.Add(new()
                    {
                        FollowerId = $"FollowerId-{i}",
                        FollowingId = userId,
                        RequestStatus = true,
                        FollowerUser = new()
                        {
                            Id = $"FollowerId-{i}",
                            Email = $"FollowerEmail{i}",
                            FullName = $"FollowerFullName{i}",
                            UserName = $"FollowerUserName{i}",
                        }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }


        public static async Task<RepositoryContext> GetDatabaseContextWithFollowings(string userId, int count = 10)
        {
            var options = new DbContextOptionsBuilder<RepositoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RepositoryContext(options);
            databaseContext.Database.EnsureCreated();
            if (!await databaseContext.Follows.AnyAsync())
            {
                for (int i = 1; i <= count; i++)
                {
                    databaseContext.Follows.Add(new()
                    {
                        FollowerId = userId,
                        FollowingId = $"FollowingId-{i}",
                        RequestStatus = true,
                        FollowingUser = new()
                        {
                            Id = $"FollowingId-{i}",
                            Email = $"FollowingEmail{i}",
                            FullName = $"FollowingFullName{i}",
                            UserName = $"FollowingUserName{i}",
                        }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }


   
    }
}
