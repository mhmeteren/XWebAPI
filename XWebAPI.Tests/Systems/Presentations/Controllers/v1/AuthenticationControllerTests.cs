

using Entities.DataTransferObjects.BaseUser;
using Entities.Exceptions.BaseUser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers.v1;
using Services.Contracts;

namespace XWebAPI.Tests.Systems.Presentations.Controllers.v1
{
    public class AuthenticationControllerTests
    {

        [Fact]
        public async Task Login_WithInvalidUser_ThrowException()
        {
            //Arrange
            BaseUserDtoForLogin loginDto = new()
            {
                Username = "Test",
                Password = "Test"
            };

            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.ValidateUser(loginDto))
                .ReturnsAsync(false);

            var mockAuthController = new AuthenticationController(mockServiceManager.Object);


            //Act
            Func<Task> act = async() => await mockAuthController.LoginUser(loginDto);

            //Assert
            await act.Should().ThrowAsync<BaseUserLoginBadRequestException>();
        }


        [Fact]
        public async Task Login_WithValidUser_ReturnTokenObject()
        {
            //Arrange
            BaseUserDtoForLogin loginDto = new()
            {
                Username = "Test",
                Password = "Test"
            };


            TokenDto tokenDto = new()
            {
                AccessToken = "TestAccessToken",
                RefreshToken = "TestRefreshToken"
            };


            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.ValidateUser(loginDto))
                .ReturnsAsync(true);

            mockServiceManager.Setup(s => s.AuthenticationService.CreateToken(true))
                .ReturnsAsync(tokenDto);

            var mockAuthController = new AuthenticationController(mockServiceManager.Object);


            //Act
            var result =  await mockAuthController.LoginUser(loginDto);

            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var token = okObjectResult.Value.Should().BeOfType<TokenDto>().Subject;
            token.Should().Be(tokenDto);
        }


    }
}
