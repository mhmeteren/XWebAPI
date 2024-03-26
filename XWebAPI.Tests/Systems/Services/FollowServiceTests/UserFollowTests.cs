
using Entities.Exceptions.BaseUser;
using Entities.Exceptions.Follow;
using Entities.Exceptions.GeneralExceptions;
using Entities.Models;
using FluentAssertions;
using Moq;
using Repositories.Contracts;
using Services.Contracts;
using Services;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services.FollowServiceTests
{
    public class UserFollowTests
    {

        [Fact]
        public async Task UserFollow_WithNonExistenLoggedInUser_ThrowException()
        {
            //Arrange

            string nonExistenUsername = "nonExistenUsername";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(nonExistenUsername))
                .ThrowsAsync(new UserNotFoundException());



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserFollow(nonExistenUsername, It.IsAny<string>());


            //Assert

            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task UserFollow_WithSelfFollowing_ThrowException()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = followingUserId
            };



            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<FollowGeneralBadRequestException>();
        }



        [Fact]
        public async Task UserFollow_WithNonExistenFollowingUser_ThrowException()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };



            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(followingUserId))
                .Throws(new UserNotFoundException());



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task UserFollow_WithUserBlockedOrBlocking_ThrowException()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            Users followingUser = new()
            {
                Id = followingUserId
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(followingUserId))
                .ReturnsAsync(followingUser);

            mockBlockedUsersService.Setup(s => s.IsBlocked(loggedInUser.Id, followingUser.Id))
                .ReturnsAsync(true);

            mockBlockedUsersService.Setup(s => s.IsBlocked(followingUser.Id, loggedInUser.Id))
                 .ReturnsAsync(true);

            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<FollowGeneralBadRequestException>();
        }


        [Fact]
        public async Task UserFollow_WithUserIsFollowed_ThrowException()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            Users followingUser = new()
            {
                Id = followingUserId
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(followingUserId))
                .ReturnsAsync(followingUser);

            mockBlockedUsersService.Setup(s => s.IsBlocked(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(loggedInUser.Id, followingUser.Id, It.IsAny<bool>()))
                .ReturnsAsync(new Follows()
                {
                    FollowerId = loggedInUser.Id,
                    FollowingId = followingUser.Id
                });


            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<IsFollowerBadRequestException>();
        }


        [Fact]
        public async Task UserFollow_WithValidUser_FollowUser()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            Users followingUser = new()
            {
                Id = followingUserId
            };

            Follows follows = new() { FollowerId = loggedInUser.Id, FollowingId = followingUser.Id };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(followingUserId))
                .ReturnsAsync(followingUser);

            mockBlockedUsersService.Setup(s => s.IsBlocked(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(loggedInUser.Id, followingUser.Id, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);


            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            await manager.UserFollow(loggedInUsername, followingUserId);


            //Assert
            mockRepositoryManager.Verify(x => x.Follows.FollowUser(
                 It.Is<Follows>(f => f.FollowerId == follows.FollowerId && f.FollowingId == follows.FollowingId)), Times.Once);

            mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);

        }

    }
}
