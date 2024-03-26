using Entities.Models;
using Entities.RequestFeatures;
using FluentAssertions;
using Moq;
using Repositories.EFCore;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Repositories
{
    public class BlockedUsersRepositoryTests
    {

        [Fact]
        public async Task GetAllBlockedUsersAsync_ShouldReturnBlockUsers()
        {
            // Arrange
            string blockerUserId = "blockerUserId";
            int blockUserCount = 5;
            var parameters = new BlockedUserParameters()
            {
                PageNumber = 1,
            };

            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            var repository = new BlockedUsersRepository(dbContext);

            // Act
            var result = await repository.GetAllBlockedUsersAsync(blockerUserId, parameters, It.IsAny<bool>());

            // Assert
            result.Should().BeOfType<PagedList<BlockedUsers>>();
            result.Count.Should().Be(blockUserCount);

            result.MetaData.PageSize.Should().Be(parameters.PageSize);
            result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
            result.MetaData.TotalCount.Should().Be(blockUserCount);

            result.ForEach(i => i.BlockerUserId.Should().Be(blockerUserId));
            result.ForEach(i => i.BlockedUserId.Should().NotBe(blockerUserId));


        }

        [Fact]
        public async Task CheckUserBlockedAsync_ShouldReturnSingleBlockUsers()
        {
            // Arrange
            string blockerUserId = "blockerUserId";
            int blockUserCount = 5;

            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            var blockedUsers = dbContext.BlockedUsers.First();
            var repository = new BlockedUsersRepository(dbContext);

            // Act
            var result = await repository.CheckUserBlockedAsync(blockedUsers.BlockerUserId, blockedUsers.BlockedUserId, It.IsAny<bool>());

            // Assert
            result.Should().BeOfType<BlockedUsers>().Should().NotBeNull();


        }


        [Fact]
        public async Task CheckUserBlockedAsync_ShouldReturnNullBlockUsers()
        {
            // Arrange
            string blockerUserId = "blockerUserId";
            int blockUserCount = 5;
            string nonExistsBlockedUserId = blockerUserId;

            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            var blockedUsers = dbContext.BlockedUsers.First();

            var repository = new BlockedUsersRepository(dbContext);

            // Act
            var result = await repository.CheckUserBlockedAsync(blockedUsers.BlockerUserId, nonExistsBlockedUserId, It.IsAny<bool>());

            // Assert
            result.Should().BeNull();
        }




        [Fact]
        public async Task BlockedUser_ShouldBlockedUser()
        {
            // Arrange
            int blockUserCount = 5;
            string blockerUserId = "blockerUserId";
            string blockedUserId = Guid.NewGuid().ToString();

            BlockedUsers blockedUser = new()
            {
                BlockerUserId = blockerUserId,
                BlockedUserId = blockedUserId,
            };


            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            var repository = new BlockedUsersRepository(dbContext);

            // Act
            repository.BlockedUser(blockedUser);
            dbContext.SaveChanges();

            // Assert
            dbContext.BlockedUsers.Count().Should().Be(blockUserCount + 1);
            dbContext.BlockedUsers.Should().Contain(blockedUser);


        }


        [Fact]
        public async Task BlockedUser_WithAlreadyBlockedUser_ThrowException()
        {
            // Arrange
            int blockUserCount = 5;
            string blockerUserId = "blockerUserId";


            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            BlockedUsers blockedUser = dbContext.BlockedUsers.First();

            var repository = new BlockedUsersRepository(dbContext);

            // Act

            Action act = () =>
            {
                repository.BlockedUser(blockedUser);
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().Throw<ArgumentException>();
        }



        [Fact]
        public async Task UnBlockedUser_ShouldDeleteBlocked()
        {
            // Arrange
            int blockUserCount = 5;
            string blockerUserId = "blockerUserId";


            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            BlockedUsers blockedUser = dbContext.BlockedUsers.First();

            var repository = new BlockedUsersRepository(dbContext);

            // Act
            repository.UnBlockedUser(blockedUser);
            dbContext.SaveChanges();

            // Assert
            dbContext.BlockedUsers.Count().Should().Be(blockUserCount - 1);
            dbContext.BlockedUsers.Should().NotContain(blockedUser);
        }


        [Fact]
        public async Task UnBlockedUser_WithNonExistsBlocked_ThrowException()
        {
            // Arrange
            int blockUserCount = 5;
            string blockerUserId = "blockerUserId";
            string blockedUserId = Guid.NewGuid().ToString();

            BlockedUsers nonExistsBlockedUser = new()
            {
                BlockerUserId = blockerUserId,
                BlockedUserId = blockedUserId,
            };


            var dbContext = await BlockedUsersFixtures.GetDatabaseContextWithBlocks(blockerUserId, blockUserCount);
            var repository = new BlockedUsersRepository(dbContext);

            // Act

            Action act = () =>
            {
                repository.UnBlockedUser(nonExistsBlockedUser);
                dbContext.SaveChanges();
            };

            // Assert
            dbContext.BlockedUsers.Should().NotContain(nonExistsBlockedUser);
            act.Should().Throw<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>();
        }




    }
}
