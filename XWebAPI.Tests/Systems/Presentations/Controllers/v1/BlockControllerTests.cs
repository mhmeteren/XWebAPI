
using Elasticsearch.Net;
using Entities.DataTransferObjects.BlockedUsers;
using Entities.DataTransferObjects.Follower;
using Entities.Exceptions.BaseUser;
using Entities.RequestFeatures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers.v1;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Presentations.Controllers.v1
{
    public class BlockControllerTests
    {

        [Fact]
        public async Task GetAllBlockedUsers_WithInValidLoggedInUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";

            BlockedUserParameters requestParameters = new()
            {
                PageNumber = 1,
            };


            var mockServiceManager = new Mock<IServiceManager>();


            mockServiceManager.Setup(s => s
                    .BlockedUsersService
                    .GetAllBlockedUsersByBlockerUserIdAsync(loggedInUsername, requestParameters, It.IsAny<bool>()))
                .Throws(new UserNotFoundException());



            var controller = new BlockController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            Func<Task> act = async () => await controller.GetAllBlockedUsers(requestParameters);


            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }



        [Fact]
        public async Task GetAllBlockedUsers_WithLoggedInUser_ReturnBlockedUsers()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";

            BlockedUserParameters requestParameters = new()
            {
                PageNumber = 1,
            };

            var pagedResponse = new PagedResponse<BlockedUserDto>(
                items: new List<BlockedUserDto>(),
            metaData: new MetaData()
            {
                CurrentPage = requestParameters.PageNumber,
                PageSize = requestParameters.PageSize,
                TotalCount = 0,
                TotalPage = 0
            });

            var mockServiceManager = new Mock<IServiceManager>();


            mockServiceManager.Setup(s => s
                    .BlockedUsersService
                    .GetAllBlockedUsersByBlockerUserIdAsync(loggedInUsername, requestParameters, It.IsAny<bool>()))
                .ReturnsAsync(pagedResponse);



            var controller = new BlockController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            var result = await controller.GetAllBlockedUsers(requestParameters);


            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var followers = okObjectResult.Value.Should().BeOfType<List<BlockedUserDto>>().Subject;

            controller.Response.Headers.Should().ContainKey("X-Pagination");

            var headerValue = controller.Response.Headers["X-Pagination"].ToString();
            headerValue.Should().Be(pagedResponse.MetaData.ToString());
        }


        [Fact]
        public async Task BlockUser_WithValidUsers_ReturnHttpCreated()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            var mockIServiceManager = new Mock<IServiceManager>();

            mockIServiceManager.Setup(s => s.BlockedUsersService.BlockedUser(loggedInUsername, blockedUserId));


            var controller = new BlockController(mockIServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };

            //Act
                
            var result = await controller.BlockUser(blockedUserId);

            //Assert

            var createdResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }


        [Fact]
        public async Task UnBlockUser_WithValidUsers_ReturnHttpNoContent()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            string blockedUserId = Guid.NewGuid().ToString();

            var mockIServiceManager = new Mock<IServiceManager>();

            mockIServiceManager.Setup(s => s.BlockedUsersService.UnBlockedUser(loggedInUsername, blockedUserId));


            var controller = new BlockController(mockIServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };

            //Act

            var result = await controller.UnBlockUser(blockedUserId);

            //Assert

            var createdResult = result.Should().BeOfType<NoContentResult>().Subject;
            createdResult.StatusCode.Should().Be(204);
        }



    }
}
