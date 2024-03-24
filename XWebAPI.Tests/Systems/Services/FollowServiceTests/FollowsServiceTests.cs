using Entities.Exceptions.Follow;
using Entities.Models;
using FluentAssertions;
using Moq;
using Repositories.Contracts;
using Services;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services.FollowServiceTests
{
    public class FollowsServiceTests
    {


        [Fact]
        public async Task IsFollower_WithFollow_ReturnTrue()
        {
            //Arrange

            string followerId = "followerId";
            string followingId = "followerId";
            Follows follows = new()
            {
                FollowingId = followingId,
                FollowerId = followerId
            };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(followerId, followingId, It.IsAny<bool>()))
                .ReturnsAsync(follows);



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            var result = await manager.IsFollower(followerId, followingId);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsFollower_WithNotFollow_ReturnFalse()
        {
            //Arrange

            string followerId = "followerId";
            string followingId = "followerId";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(followerId, followingId, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            var result = await manager.IsFollower(followerId, followingId);

            //Assert
            result.Should().BeFalse();
        }



        [Fact]
        public async Task CheckFollowerAsync_WithFollowingCase_ReturnFollowsObject()
        {
            //Arrange

            string followerId = "followerId";
            string followingId = "followerId";
            Follows follows = new()
            {
                FollowingId = followingId,
                FollowerId = followerId
            };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(followerId, followingId, It.IsAny<bool>()))
                .ReturnsAsync(follows);



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            var result = await manager.CheckFollowerAsync(followerId, followingId);

            //Assert
            result.Should().BeOfType<Follows>();
            result.Should().Be(follows);
        }



        [Fact]
        public async Task CheckFollowerAsync_WithNotFollowingCase_ThrowException()
        {
            //Arrange

            string followerId = "followerId";
            string followingId = "followerId";


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(followerId, followingId, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.CheckFollowerAsync(followerId, followingId);

            //Assert
            await act.Should().ThrowAsync<UnFollowerBadRequestException>();
        }



    }
}
