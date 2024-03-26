
using Entities.DataTransferObjects.BlockedUsers;
using Entities.DataTransferObjects.Follower;
using Entities.Exceptions.BaseUser;
using Entities.Exceptions.Block;
using Entities.Exceptions.GeneralExceptions;
using Entities.Models;
using Entities.RequestFeatures;
using FluentAssertions;
using Moq;
using Repositories.Contracts;
using Services;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services.BlockedUsersServiceTests
{
    public class BlockedUsersServiceTests
    {

        [Fact]
        public async Task GetAllBlockedUsersByBlockerUserIdAsync_ReturnBlockedUsers()
        {

            //Arrange
            string loggedInUsername = "loggedInUsername";
            BlockedUserParameters requestParameters = new() { PageNumber = 1 };

            Users loggedInUser = new()
            {
                Id = "loggedInUserId",
            };


            var pagedList = PagedList<BlockedUsers>
             .ToPagedList(new List<BlockedUsers>(), requestParameters);


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockRepositoryManager.Setup(r => r.BlockedUsers.GetAllBlockedUsersAsync(loggedInUser.Id, requestParameters, It.IsAny<bool>()))
                .ReturnsAsync(pagedList);


            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);



            //Act
            var result = await manager.GetAllBlockedUsersByBlockerUserIdAsync(loggedInUsername, requestParameters, It.IsAny<bool>());


            //Assert
            var pagedResponse = result.Should().BeOfType<PagedResponse<BlockedUserDto>>().Subject;
            pagedResponse.MetaData.Should().Be(pagedList.MetaData);
            pagedResponse.Items.Should().BeEquivalentTo(mapper.Map<IEnumerable<BlockedUserDto>>(pagedList));
        }



        [Fact]
        public async Task IsBlocked_WithNonExistsBlockedUser_ReturnFalse()
        {
            //Arrange
            string blockerUserId = "blockerUserId";
            string blockedUserId = "blockedUserId";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();

            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()))
                .ReturnsAsync((BlockedUsers)null);

            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);


            //Act
            var result = await manager.IsBlocked(blockerUserId, blockedUserId);

            //Assert
            mockRepositoryManager.Verify(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()), Times.Once);
            result.Should().BeFalse();
        }


        [Fact]
        public async Task IsBlocked_WithExistsBlockedUser_ReturnTrue()
        {
            //Arrange
            string blockerUserId = "blockerUserId";
            string blockedUserId = "blockedUserId";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();

            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()))
                .ReturnsAsync(new BlockedUsers() { BlockerUserId = blockerUserId, BlockedUserId = blockedUserId});

            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);


            //Act
            var result = await manager.IsBlocked(blockerUserId, blockedUserId);

            //Assert
            mockRepositoryManager.Verify(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()), Times.Once);
            result.Should().BeTrue();
        }





        [Fact]
        public async Task CheckUserBlockedAsync_WithNonExistsBlockedUser_ThrowException()
        {
            //Arrange
            string blockerUserId = "blockerUserId";
            string blockedUserId = "blockedUserId";

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();

            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()))
                .ReturnsAsync((BlockedUsers)null);

            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);


            //Act
            Func<Task> act = async() => await manager.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>());

            //Assert
            await act.Should().ThrowAsync<UnBlockedBadRequestException>();
        }

        [Fact]
        public async Task CheckUserBlockedAsync_WithExistsBlockedUser_ReturnBlockedUser()
        {
            //Arrange
            string blockerUserId = "blockerUserId";
            string blockedUserId = "blockedUserId";

            var blockedUser = new BlockedUsers() { BlockerUserId = blockerUserId, BlockedUserId = blockedUserId };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();

            mockRepositoryManager.Setup(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()))
                .ReturnsAsync(blockedUser);

            var manager = new BlockedUsersManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object);


            //Act
            var result = await manager.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>());

            //Assert
            mockRepositoryManager.Verify(r => r.BlockedUsers.CheckUserBlockedAsync(blockerUserId, blockedUserId, It.IsAny<bool>()), Times.Once);
            result.Should().Be(blockedUser);
        }

    }
}
