using Entities.DataTransferObjects.BaseUser;
using Entities.Exceptions.BaseUser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers.v1;
using Presentation.Validators.Authentication;
using Services.Contracts;

namespace XWebAPI.Tests.Systems.Presentations.Controllers.v1
{
    public class AuthenticationControllerTests
    {

        #region Login Tests
        [Fact]
        public async Task Login_WithInvalidUser_ThrowException()
        {
            //Arrange
            BaseUserDtoForLogin loginDto = new(
                Username : "ValidUsername",
                Password : "ValisP@ssw0rd!"
            );

            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.ValidateUser(loginDto))
                .ReturnsAsync(false);
        
            var mockAuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new LoginValidator();


            //Act
            Func<Task> act = async() => await mockAuthController.LoginUser(loginDto, validator);

            //Assert
            await act.Should().ThrowAsync<BaseUserLoginBadRequestException>();
        }



        [Fact]
        public async Task Login_WithInvalidUserData_ReturnBadRequestByFluentValidation()
        {
            //Arrange
            BaseUserDtoForLogin loginDto = new(
                Username: " Invalid username ",
                Password: ""
            );

            var mockServiceManager = new Mock<IServiceManager>();
            var AuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new LoginValidator();


            //Act
            var result =  await AuthController.LoginUser(loginDto, validator);

            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            AuthController.ModelState.Keys.Should().Contain("Username");
            AuthController.ModelState.Keys.Should().Contain("Password");
        }


        [Fact]
        public async Task Login_WithValidUser_ReturnTokenObject()
        {
            //Arrange
            BaseUserDtoForLogin loginDto = new(
                Username: "ValidUsername",
                Password: "ValisP@ssw0rd!"
            );


            TokenDto tokenDto = new
            (
               AccessToken: "TestAccessToken",
               RefreshToken : "TestRefreshToken"
            );


            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.ValidateUser(loginDto))
                .ReturnsAsync(true);

            mockServiceManager.Setup(s => s.AuthenticationService.CreateToken(true))
                .ReturnsAsync(tokenDto);

            var mockAuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new LoginValidator();

            //Act
            var result =  await mockAuthController.LoginUser(loginDto, validator);

            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var token = okObjectResult.Value.Should().BeOfType<TokenDto>().Subject;
            token.Should().Be(tokenDto);
        }
        #endregion Login Tests



        #region Refresh Token Tests

        [Fact]
        public async Task Refresh_WithInvalidToken_ReturnBadRequestByFluentValidation()
        {
            //Arrange
            TokenDto tokenDto = new(
                AccessToken: null,
                RefreshToken: null
            );

            var mockServiceManager = new Mock<IServiceManager>();

            var AuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new TokenRefreshValidator();


            //Act
            var result = await AuthController.Refresh(tokenDto, validator);

            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            AuthController.ModelState.Keys.Should().Contain("AccessToken");
            AuthController.ModelState.Keys.Should().Contain("RefreshToken");
        }


        [Fact]
        public async Task Refresh_WithInvalidUser_ThrowException()
        {
            //Arrange
            TokenDto tokenDto = new(
                AccessToken: "testAccessToken",
                RefreshToken: "testRefreshToken"
            );



            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.RefreshToken(tokenDto))
                .ThrowsAsync(new RefreshTokenBadRequestException());

            var mockAuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new TokenRefreshValidator();


            //Act
            Func<Task> act = async () => await mockAuthController.Refresh(tokenDto, validator);

            //Assert
            await act.Should().ThrowAsync<RefreshTokenBadRequestException>();
        }



        [Fact]
        public async Task Refresh_WithValidUser_ReturnTokenObject()
        {
            //Arrange
            TokenDto requestTokenDto = new(
                AccessToken: "testAccessToken",
                RefreshToken: "testRefreshToken"
            );

            TokenDto responseTokenDto = new(
                AccessToken: "responseTestAccessToken",
                RefreshToken: "responseTestRefreshToken"
            );

            var mockServiceManager = new Mock<IServiceManager>();
            mockServiceManager.Setup(s => s.AuthenticationService.RefreshToken(requestTokenDto))
                .ReturnsAsync(responseTokenDto);


            var mockAuthController = new AuthenticationController(mockServiceManager.Object);
            var validator = new TokenRefreshValidator();

            //Act
            var result = await mockAuthController.Refresh(requestTokenDto, validator);

            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var token = okObjectResult.Value.Should().BeOfType<TokenDto>().Subject;
            token.Should().Be(responseTokenDto);
        }

        #endregion Refresh Token Tests

    }
}
