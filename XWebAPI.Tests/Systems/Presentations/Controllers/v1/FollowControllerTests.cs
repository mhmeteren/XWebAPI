using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers.v1;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Presentations.Controllers.v1
{
    public class FollowControllerTests
    {

        [Fact]
        public async Task FollowUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
                    .FollowsService
                    .UserFollow(loggedInUsername, It.IsAny<string>()))
                .Throws(new Exception());



            var controller = new FollowController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            Func<Task> act = async () => await controller.FollowUser(It.IsAny<string>());


            //Assert
            await act.Should().ThrowAsync<Exception>();
        }


        [Fact]
        public async Task FollowUser_WithValidUsers_ReturnHttpCreated()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.FollowsService.UserFollow(loggedInUsername, It.IsAny<string>()));

            var controller = new FollowController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            var result = await controller.FollowUser(It.IsAny<string>());


            //Assert
            var createdResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }




        [Fact]
        public async Task UnFollowUser_WithValidUsers_ReturnHttpNoContent()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.FollowsService.UserUnFollow(loggedInUsername, It.IsAny<string>()));

            var controller = new FollowController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            var result = await controller.UnFollowUser(It.IsAny<string>());


            //Assert
            var createdResult = result.Should().BeOfType<NoContentResult>().Subject;
            createdResult.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteFollower_WithValidUsers_ReturnHttpNoContent()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.FollowsService.DeleteFollower(loggedInUsername, It.IsAny<string>()));

            var controller = new FollowController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };


            //Act
            var result = await controller.DeleteFollower(It.IsAny<string>());


            //Assert
            var createdResult = result.Should().BeOfType<NoContentResult>().Subject;
            createdResult.StatusCode.Should().Be(204);
        }



    }
}
