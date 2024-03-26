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
    public class UnBlockedUserTests
    {

        [Fact]
        public async Task UnBlockedUser_WithNonExistenLoggedInUser_ThrowException()
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
            Func<Task> act = async () => await manager.UnBlockedUser(loggedInUsername, It.IsAny<string>());

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task UnBlockedUser_WithSelfUnBlocked_ThrowException()
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
            Func<Task> act = async () => await manager.UnBlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<BlockGeneralBadRequestException>();
        }



        [Fact]
        public async Task UnBlockedUser_WithNonExistenBlockedUser_ThrowException()
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
            Func<Task> act = async () => await manager.UnBlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }



        [Fact]
        public async Task UnBlockedUser_WithNonBlockedUser_ThrowException()
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
                .Throws(new UnBlockedBadRequestException());


            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            Func<Task> act = async () => await manager.UnBlockedUser(loggedInUsername, blockedUserId);

            //Assert
            await act.Should().ThrowAsync<UnBlockedBadRequestException>();
        }


        [Fact]
        public async Task UnBlockedUser_WithValidUser_UnBlockedUser()
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
            await manager.UnBlockedUser(loggedInUsername, blockedUserId);

            //Assert

            mockUserService.Verify(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername), Times.Once);
            mockUserService.Verify(s => s.GetUserByIdCheckAndExistsAsync(blockedUserId), Times.Once);

            mockRepositoryManager.Verify(r => r.BlockedUsers.CheckUserBlockedAsync(loggedInUser.Id, blockedUser.Id, It.IsAny<bool>()), Times.Once);

            mockRepositoryManager.Verify(r => r.BlockedUsers.UnBlockedUser(
                It.Is<BlockedUsers>(b => b.BlockerUserId == loggedInUser.Id && b.BlockedUserId == blockedUser.Id)), Times.Once);

            mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
        }
       

    }
}
