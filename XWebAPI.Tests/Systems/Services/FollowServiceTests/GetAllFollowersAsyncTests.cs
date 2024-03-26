using Entities.DataTransferObjects.Follower;
using Entities.Exceptions.BaseUser;
using Entities.Exceptions.User;
using Entities.Models;
using Entities.RequestFeatures;
using FluentAssertions;
using Moq;
using Repositories.Contracts;
using Services.Contracts;
using Services;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services.FollowServiceTests
{
    public class GetAllFollowersAsyncTests
    {


        [Fact]
        public async Task GetAllFollowersAsync_WithInValidUsername_ThrowException()
        {
            //Arrange

            string inValidUsername = "inValidUsername";
            FollowParameters parameters = new()
            {
                PageNumber = 1
            };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(inValidUsername))
                .ThrowsAsync(new UserInvalidValueBadRequestException());



            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.GetAllFollowersAsync(inValidUsername, It.IsAny<string>(), parameters, It.IsAny<bool>());


            //Assert

            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();
        }


        [Fact]
        public async Task GetAllFollowersAsync_WithNonExistenUsername_ThrowException()
        {
            //Arrange

            string nonExistenUsername = "nonExistenUsername";
            FollowParameters parameters = new()
            {
                PageNumber = 1
            };

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

            Func<Task> act = async () => await manager.GetAllFollowersAsync(nonExistenUsername, It.IsAny<string>(), parameters, It.IsAny<bool>());


            //Assert

            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task GetAllFollowersAsync_WithInValidLoggedInUser_ThrowException()
        {
            //Arrange

            string validUsername = "validUsername";
            string loggedInUsername = "loggedInUsername";
            FollowParameters parameters = new()
            {
                PageNumber = 1
            };

            Users user = new()
            {
                Id = "UserId",
                UserName = validUsername,
                IsPrivateAccount = true
            };


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(validUsername))
                .ReturnsAsync(user);

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                 .Throws(new UserNotFoundException());


            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);

            //Act

            Func<Task> act = async () => await manager.GetAllFollowersAsync(validUsername, loggedInUsername, parameters, It.IsAny<bool>());


            //Assert

            await act.Should().ThrowAsync<UserNotFoundException>();
        }




        [Fact]
        public async Task GetAllFollowersAsync_WithIsPrivateAccountAndIsNotFollower_ThrowException()
        {
            //Arrange

            string validUsername = "validUsername";
            string loggedInUsername = "loggedInUsername";
            FollowParameters parameters = new()
            {
                PageNumber = 1
            };

            Users user = new()
            {
                Id = "UserId",
                UserName = validUsername,
                IsPrivateAccount = true
            };

            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(validUsername))
                .ReturnsAsync(user);

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                 .ReturnsAsync(loggedInUser);

            mockRepositoryManager.Setup(x => x.Follows.CheckUserFollowingAsync(loggedInUser.Id, user.Id, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);


            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);


            //Act

            Func<Task> act = async () => await manager.GetAllFollowersAsync(validUsername, loggedInUsername, parameters, It.IsAny<bool>());



            //Assert

            await act.Should().ThrowAsync<UserAccountIsPrivateBadRequestException>();
        }


        [Fact]
        public async Task GetAllFollowersAsync_WithIsNotPrivateAccount_ReturnFollowerDtos()
        {
            //Arrange

            string validUsername = "validUsername";
            string loggedInUsername = "loggedInUsername";
            FollowParameters parameters = new()
            {
                PageNumber = 1
            };

            Users user = new()
            {
                Id = "UserId",
                UserName = validUsername,
                IsPrivateAccount = false
            };

            Users loggedInUser = new()
            {
                Id = "loggedInUserId"
            };

            var pagedList = PagedList<Follows>
                .ToPagedList(new List<Follows>(), parameters);


            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var mockUserService = new Mock<IUserService>();
            var mockBlockedUsersService = new Mock<IBlockedUsersService>();


            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(validUsername))
                .ReturnsAsync(user);

            mockUserService.Setup(s => s.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername))
                 .ReturnsAsync(loggedInUser);

            mockRepositoryManager.Setup(x => x.Follows.CheckUserFollowingAsync(loggedInUser.Id, user.Id, It.IsAny<bool>()))
                .ReturnsAsync((Follows)null);


            mockRepositoryManager.Setup(x => x.Follows.GetAllFollowersAsync(user.Id, parameters, It.IsAny<bool>()))
                .ReturnsAsync(pagedList);


            var manager = new FollowsManager(
                mockRepositoryManager.Object,
                mapper,
                mockUserService.Object,
                mockBlockedUsersService.Object);


            //Act

            var result = await manager.GetAllFollowersAsync(validUsername, loggedInUsername, parameters, It.IsAny<bool>());



            //Assert

            var pagedResponse = result.Should().BeOfType<PagedResponse<FollowerDto>>().Subject;
            pagedResponse.MetaData.Should().Be(pagedList.MetaData);
            pagedResponse.Items.Should().BeEquivalentTo(mapper.Map<IEnumerable<FollowerDto>>(pagedList));
        }

    }
}
