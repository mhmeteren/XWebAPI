
using Microsoft.EntityFrameworkCore;
using Repositories.EFCore;

namespace XWebAPI.Tests.Fixtures
{
    public static class BlockedUsersFixtures
    {

        public static async Task<RepositoryContext> GetDatabaseContextWithBlocks(string BlockerUserId, int count = 10)
        {
            var options = new DbContextOptionsBuilder<RepositoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RepositoryContext(options);
            databaseContext.Database.EnsureCreated();
            if (!await databaseContext.BlockedUsers.AnyAsync())
            {
                for (int i = 1; i <= count; i++)
                {
                    databaseContext.BlockedUsers.Add(new()
                    {
                        BlockerUserId = BlockerUserId,
                        BlockedUserId = $"BlockedUserId-{i}",
                        BlockedUser = new()
                        {
                            Id = $"BlockedUserId-{i}",
                            Email = $"BlockedUserEmail{i}",
                            FullName = $"BlockedUserFullName{i}",
                            UserName = $"BlockedUserUserName{i}",
                        }
                        
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

    }
}
