
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
    public class UserUnFollowTests
    {

        [Fact]
        public async Task UserUnFollow_WithNonExistenLoggedInUser_ThrowException()
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

            Func<Task> act = async () => await manager.UserUnFollow(nonExistenUsername, It.IsAny<string>());


            //Assert

            await act.Should().ThrowAsync<UserNotFoundException>();
        }



        [Fact]
        public async Task UserUnFollow_WithSelfUnFollowing_ThrowException()
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

            Func<Task> act = async () => await manager.UserUnFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<FollowGeneralBadRequestException>();
        }



        [Fact]
        public async Task UserUnFollow_WithNotFollowingUser_ThrowException()
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

            mockRepositoryManager.Setup(m => m.Follows.CheckUserFollowingAsync(loggedInUser.Id, followingUserId, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);

            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.UserUnFollow(loggedInUsername, followingUserId);


            //Assert

            await act.Should().ThrowAsync<UnFollowerBadRequestException>();
        }


        [Fact]
        public async Task UserUnFollow_WithFollowingUser_UnFollow()
        {
            //Arrange

            string loggedInUsername = "loggedInUsername";
            string followingUserId = "followingUserId";
            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            Follows follows = new()
            {
                FollowingId = followingUserId,
                FollowerId = loggedInUser.Id
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                .ReturnsAsync(loggedInUser);

            mockRepositoryManager.Setup(m => m.Follows.CheckUserFollowingAsync(loggedInUser.Id, followingUserId, It.IsAny<bool>()))
                .ReturnsAsync(follows);

            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            await manager.UserUnFollow(loggedInUsername, followingUserId);


            //Assert

            mockRepositoryManager.Verify(x => x.Follows.UnFollowUser(
                It.Is<Follows>(f => f.FollowerId == follows.FollowerId && f.FollowingId == follows.FollowingId)), Times.Once);

            mockRepositoryManager.Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}
