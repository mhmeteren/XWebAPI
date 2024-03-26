using Entities.Models;
using Entities.RequestFeatures;
using FluentAssertions;
using Moq;
using Repositories.EFCore;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Repositories
{
    public class FollowsRepositoryTests
    {

        [Fact]
        public async Task GetAllFollowersAsync_ShouldReturnFollowers()
        {
            // Arrange
            string userId = "userId";
            int followsCount = 5;
            var parameters = new FollowParameters()
            {
                PageNumber = 1,
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowers(userId, followsCount);
            var repository = new FollowsRepository(dbContext);

            // Act
            var result = await repository.GetAllFollowersAsync(userId, parameters, It.IsAny<bool>());

            // Assert
            result.Should().BeOfType<PagedList<Follows>>();
            result.Count.Should().Be(followsCount);

            result.MetaData.PageSize.Should().Be(parameters.PageSize);
            result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
            result.MetaData.TotalCount.Should().Be(followsCount);

            result.ForEach(i => i.FollowingId.Should().Be(userId));
            result.ForEach(i => i.RequestStatus.Should().BeTrue());

        }


        [Fact]
        public async Task GetAllFollowingsAsync_ShouldReturnFollowings()
        {
            // Arrange
            string userId = "userId";
            int followingCount = 5;
            var parameters = new FollowParameters()
            {
                PageNumber = 1,
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowings(userId, followingCount);

            var repository = new FollowsRepository(dbContext);

            // Act
            var result = await repository.GetAllFollowingsAsync(userId, parameters, It.IsAny<bool>());

            // Assert
            result.Should().BeOfType<PagedList<Follows>>();
            result.Count.Should().Be(followingCount);

            result.MetaData.PageSize.Should().Be(parameters.PageSize);
            result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
            result.MetaData.TotalCount.Should().Be(followingCount);

            result.ForEach(i => i.FollowerId.Should().Be(userId));
            result.ForEach(i => i.RequestStatus.Should().BeTrue());

        }



        [Fact]
        public async Task CheckUserFollowingAsync_ShouldReturnSingleFollows()
        {
            // Arrange
            string userId = "userId";
            int followingCount = 5;
            var parameters = new FollowParameters()
            {
                PageNumber = 1,
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowings(userId, followingCount);
            var testFollows = dbContext.Follows.First();
            var repository = new FollowsRepository(dbContext);

            // Act
            var result = await repository.CheckUserFollowingAsync(testFollows.FollowerId, testFollows.FollowingId, It.IsAny<bool>());

            // Assert
            result.Should().BeOfType<Follows>().Should().NotBeNull();
        }


        [Fact]
        public async Task CheckUserFollowingAsync_ShouldReturnNullFollows()
        {
            // Arrange
            string userId = "userId";
            int followingCount = 5;
            var parameters = new FollowParameters()
            {
                PageNumber = 1,
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowings(userId, followingCount);
            var testFollows = dbContext.Follows.First();
            string nonExistsFollowingUserId = userId;

            var repository = new FollowsRepository(dbContext);

            // Act
            var result = await repository.CheckUserFollowingAsync(testFollows.FollowerId, nonExistsFollowingUserId, It.IsAny<bool>());

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public async Task FollowUser_ShouldBeCreateFollow()
        {
            // Arrange
            string userId = "userId";
            string FollowerUserId = Guid.NewGuid().ToString();
            int followsCount = 5;

            Follows createFollow = new()
            {
                FollowerId = FollowerUserId,
                FollowingId = userId
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowers(userId, followsCount);
            var repository = new FollowsRepository(dbContext);

            // Act
            repository.FollowUser(createFollow);
            dbContext.SaveChanges();

            // Assert
            dbContext.Follows.Count().Should().Be(followsCount + 1);
            dbContext.Follows.Should().Contain(createFollow);

        }


        [Fact]
        public async Task FollowUser_WithAlreadyCreatedFollow_ThrowException()
        {
            // Arrange
            string userId = "userId";
            string FolloerUserId = Guid.NewGuid().ToString();
            int followsCount = 5;


            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowers(userId, followsCount);
            var repository = new FollowsRepository(dbContext);

            // Act
            Action act = () =>
            {
                repository.FollowUser(dbContext.Follows.First());
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().Throw<ArgumentException>();
        }



        [Fact]
        public async Task UnFollowUser_ShouldBeDeleteFollow()
        {
            // Arrange
            string userId = "userId";
            int followsCount = 5;

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowers(userId, followsCount);
            var testFollowing = dbContext.Follows.First();
            var repository = new FollowsRepository(dbContext);

            // Act
            repository.UnFollowUser(testFollowing);
            dbContext.SaveChanges();

            // Assert
            dbContext.Follows.Count().Should().Be(followsCount - 1);
            dbContext.Follows.Should().NotContain(testFollowing);
        }



        [Fact]
        public async Task UnFollowUser_WithNonExistsFollows_ThrowException()
        {
            // Arrange
            string userId = "userId";
            string FollowerUserId = Guid.NewGuid().ToString();
            int followsCount = 5;

            Follows nonExistsFollowing = new()
            {
                FollowerId = FollowerUserId,
                FollowingId = userId
            };

            var dbContext = await FollowsFixtures.GetDatabaseContextWithFollowers(userId, followsCount);
            var repository = new FollowsRepository(dbContext);

            // Act
            Action act = () =>
            {
                repository.UnFollowUser(nonExistsFollowing);
                dbContext.SaveChanges();
            };

            // Assert
            dbContext.Follows.Should().NotContain(nonExistsFollowing);
            act.Should().Throw<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>();
        }


    }
}
