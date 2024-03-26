

using Entities.Exceptions.BaseUser;
using Entities.Exceptions.Block;
using Entities.Exceptions.GeneralExceptions;
using Entities.Models;
using FluentAssertions;
using Moq;
using Repositories.Contracts;
using Services.Contracts;
using Services;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services.BlockedUsersServiceTests
{
    public class BlockedUserTests
    {

        [Fact]
        public async Task BlockedUser_WithNonExistenLoggedInUser_ThrowException()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ThrowsAsync(new UserNotFoundException());



            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            Func<Task> act = async () => await manager.BlockedUser(loggedInUsername, It.IsAny<string>());

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task BlockedUser_WithSelfBlocked_ThrowException()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            Users loggedInUser = new()
            {
                Id = blockedUserId,
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);



            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            Func<Task> act = async () => await manager.BlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<BlockGeneralBadRequestException>();
        }



        [Fact]
        public async Task BlockedUser_WithNonExistenBlockingUser_ThrowException()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            Users loggedInUser = new()
            {
                Id = "loggedInUserId",
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(blockedUserId))
                .Throws(new UserNotFoundException());


            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            Func<Task> act = async () => await manager.BlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task BlockedUser_WithAlreadyBlockedUser_ThrowException()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            Users loggedInUser = new()
            {
                Id = "loggedInUserId",
            };

            Users blockedUser = new()
            {
                Id = blockedUserId,
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(blockedUserId))
                .ReturnsAsync(blockedUser);


            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()))
                .ReturnsAsync(new BlockedUsers() { BlockerUserId = loggedInUser.Id, BlockedUserId = blockedUser.Id });


            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            Func<Task> act = async () => await manager.BlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<IsBlockedBadRequestException>();
        }


        [Fact]
        public async Task BlockedUser_WithValidUser_BlockedUserAndDeleteFollows()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            Users loggedInUser = new()
            {
                Id = "loggedInUserId",
            };

            Users blockedUser = new()
            {
                Id = blockedUserId,
            };

            Follows following = new() { FollowerId = loggedInUser.Id, FollowingId = blockedUser.Id };
            Follows follower = new() { FollowingId = loggedInUser.Id, FollowerId = blockedUser.Id };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockUserService.Setup(s => s.GetUserByIdCheckAndExistsAsync(blockedUserId))
                .ReturnsAsync(blockedUser);


            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()))
                .ReturnsAsync((BlockedUsers)null);

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()))
                .ReturnsAsync(following);

            mockRepositoryManager.Setup(r => r.Follows.CheckUserFollowingAsync(blockedUser.Id, loggedInUser.Id, It.IsAny<bool>()))
                .ReturnsAsync(follower);


            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            await manager.BlockedUser(loggedInUsername, blockedUserId);

            //Assert

            mockUserService.Verify(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername), Times.Once);
            mockUserService.Verify(s => s.GetUserByIdCheckAndExistsAsync(blockedUserId), Times.Once);

            mockRepositoryManager.Verify(r => r.BlockedUsers.CheckUserBlockedAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()), Times.Once);
            mockRepositoryManager.Verify(r => r.Follows.CheckUserFollowingAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()), Times.Once);
            mockRepositoryManager.Verify(r => r.Follows.CheckUserFollowingAsync(blockedUser.Id, loggedInUser.Id, It.IsAny<bool>()), Times.Once);


            mockRepositoryManager.Verify(x => x.Follows.UnFollowUser(
                 It.Is<Follows>(f => f.FollowerId == following.FollowerId && f.FollowingId == following.FollowingId)), Times.Once);

            mockRepositoryManager.Verify(x => x.Follows.UnFollowUser(
                 It.Is<Follows>(f => f.FollowerId == follower.FollowerId && f.FollowingId == follower.FollowingId)), Times.Once);

            mockRepositoryManager.Verify(r => r.BlockedUsers.BlockedUser(
                It.Is<BlockedUsers>(b => b.BlockerUserId == loggedInUser.Id && b.BlockedUserId == blockedUser.Id)), Times.Once);

            mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
        }
     

    }
}
