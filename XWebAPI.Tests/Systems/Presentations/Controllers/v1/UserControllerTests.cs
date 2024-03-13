using Services.Contracts;
using Moq;
using Entities.Exceptions.User;
using Presentation.Controllers.v1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Entities.DataTransferObjects.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Entities.Exceptions.BaseUser;
using XWebAPI.Tests.Fixtures;
using Entities.DataTransferObjects.Follower;
using Entities.RequestFeatures;


namespace XWebAPI.Tests.Systems.Presentations.Controllers.v1
{
    public class UserControllerTests
    {
        #region GetUserByUsername
        [Fact]
        public async Task GetUserByUsername_WithInvalidUsernameLenght_ThrowException()
        {
            //Arrange
            string InvalidUsername = "a";
            var mockService = new Mock<IServiceManager>();
            mockService.Setup(svc => svc.UserService.GetUserProfileByUsernameAsync(InvalidUsername))
                .ThrowsAsync(new UserInvalidValueBadRequestException());

            var controller = new UserController(mockService.Object);

            //Act

            Func<Task> act = async () => await controller.GetUserByUsername(InvalidUsername);

            //Assert
            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();

        }


        [Fact]
        public async Task GetUserByUsername_WithInvalidUsernameWithWhiteSpaces_ThrowException()
        {
            //Arrange
            string InvalidUsername = "   a     ";
            var mockService = new Mock<IServiceManager>();
            mockService.Setup(svc => svc.UserService.GetUserProfileByUsernameAsync(InvalidUsername))
                .ThrowsAsync(new UserInvalidValueBadRequestException());

            var controller = new UserController(mockService.Object);

            //Act

            Func<Task> act = async () => await controller.GetUserByUsername(InvalidUsername);

            //Assert
            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();

        }



        [Fact]
        public async Task GetUserByUsername_WithNonExistentUsername_ReturnNull()
        {
            //Arrange
            var mockService = new Mock<IServiceManager>();
            mockService.Setup(svc => svc.UserService.GetUserProfileByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var controller = new UserController(mockService.Object);

            //Act
            var result = await controller.GetUserByUsername(It.IsAny<string>());

            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeNull();
        }




        [Fact]
        public async Task GetUserByUsername_WithValidAndExistentUsername_ReturnUserDto()
        {
            //Arrange
            UserProfileDto userDto = new()
            {
                UserName = "UserName",
                FullName = "FullName",
                About = "test",
                BackgroundImageUrl = "/image",
                ProfileImageUrl = "/image"
            };

            var mockService = new Mock<IServiceManager>();
            mockService.Setup(s => s.UserService.GetUserProfileByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(userDto);


            var controller = new UserController(mockService.Object);

            //Act

            var result = await controller.GetUserByUsername(It.IsAny<string>());

            //Assert
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var user = okObjectResult.Value.Should().BeOfType<UserProfileDto>().Subject;
            user.Should().NotBeNull();

        }

        #endregion GetUserByUsername





        #region RegisterUser

        [Fact]
        public async Task RegisterUser_WithInValidUserObject_ReturnBadRequest()
        {

            //Arrange

            var userDto = new UserDtoForRegister
            {
                FullName = "u",
                Email = "test",
                UserName = "u",
                Birthday = new DateTime(1, 1, 1),
                Password = "p"
            };


            var failedPasswordPolicyResult = IdentityResult.Failed(
                new IdentityError { Code = "FullName", Description = "Invalid FullName" },
                new IdentityError { Code = "Email", Description = "Invalid Email" },
                new IdentityError { Code = "UserName", Description = "Invalid UserName" },
                new IdentityError { Code = "Birthday", Description = "Invalid Birthday" },
                new IdentityError { Code = "Password", Description = "Password does not meet the policy requirements" });

            var mockService = new Mock<IServiceManager>();
            mockService.Setup(s => s.UserService.ReqisterUserAsync(userDto))
                .ReturnsAsync(failedPasswordPolicyResult);


            var controller = new UserController(mockService.Object);

            //Act

            var result = await controller.RegisterUser(userDto);


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            var error = badRequestResult.Value as SerializableError;
            error.Should().NotBeNull();

            (error["FullName"] as string[]).Should().Contain("Invalid FullName");
            (error["Email"] as string[]).Should().Contain("Invalid Email");
            (error["UserName"] as string[]).Should().Contain("Invalid UserName");
            (error["Birthday"] as string[]).Should().Contain("Invalid Birthday");
            (error["Password"] as string[]).Should().Contain("Password does not meet the policy requirements");
        }


        [Fact]
        public async Task RegisterUser_WithValidUserObject_ReturnHttpCreated()
        {

            //Arrange

            var userDto = new UserDtoForRegister
            {
                FullName = "TestFullName",
                Email = "test@test.com",
                UserName = "Testusername",
                Birthday = new DateTime(1990, 1, 1),
                Password = "ValIdP@ssw0rd!123"
            };



            var mockService = new Mock<IServiceManager>();
            mockService.Setup(s => s.UserService.ReqisterUserAsync(userDto))
                .ReturnsAsync(IdentityResult.Success);


            var controller = new UserController(mockService.Object);

            //Act   
            var result = await controller.RegisterUser(userDto);


            //Assert
            var createdResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }
        #endregion RegisterUser





        #region UserUpdate

        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_Return200HttpOk()
        {

            //Arrange
            var fake_username = "testUser";

            var fakeClaims = new List<Claim>
            {
                new(ClaimTypes.Name, fake_username),
            };

            var identity = new ClaimsIdentity(fakeClaims, It.IsAny<string>());
            var user = new ClaimsPrincipal(identity);


            var mockHttpContext = new DefaultHttpContext
            {
                User = user
            };

            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(fake_username, It.IsAny<UserDtoForAccountUpdate>()))
                    .ReturnsAsync(IdentityResult.Success);


            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext,
                }
            };



            //Act
            var result = await userController.UserUpdateProfile(It.IsAny<UserDtoForAccountUpdate>());


            //Assert
            var createdResult = result.Should().BeOfType<OkObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_Return400BadRequest()
        {

            //Arrange
            var fake_username = "testUser";

            var mockServiceManager = new Mock<IServiceManager>();

            var mockIdentityFailed = IdentityResult.Failed(
                    new IdentityError { Code = "test", Description = "testfaledmsq" });

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(fake_username, It.IsAny<UserDtoForAccountUpdate>()))
                    .ReturnsAsync(mockIdentityFailed);


            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = UserFixtures.GetMockControllerContext(fake_username),
                }
            };


            //Act
            var result = await userController.UserUpdateProfile(It.IsAny<UserDtoForAccountUpdate>());


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
        }


        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_ThrowUserNotFoundException()
        {

            //Arrange
            var fake_username = "testUser";

            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(fake_username, It.IsAny<UserDtoForAccountUpdate>()))
                    .ThrowsAsync(new UserNotFoundException());


            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = UserFixtures.GetMockControllerContext(fake_username),
                }
            };


            //Act
            Func<Task> act = async () => await userController.UserUpdateProfile(It.IsAny<UserDtoForAccountUpdate>());


            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }
        #endregion UserUpdate





        #region Follow


        [Fact]
        public async Task GetAllFollowersByUserName_WithInvalidUsername_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowersAsync(It.IsAny<string>(), loggedInUsername, requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserInvalidValueBadRequestException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowersByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();
        }


        [Fact]
        public async Task GetAllFollowersByUserName_WithInvalidUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowersAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserNotFoundException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowersByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task GetAllFollowersByUserName_WithValidUser_ReturnFollowerDtos()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            string testUserId = Guid.NewGuid().ToString();
            FollowParameters requestParameters = new() { PageNumber = 1 };

            var pagedResponse = new PagedResponse<FollowerDto>(
                items: new List<FollowerDto>(),
                metaData: new MetaData()
                {
                    CurrentPage = requestParameters.PageNumber,
                    PageSize = requestParameters.PageSize,
                    TotalCount = 0,
                    TotalPage = 0
                });

            var mockServiceManager = new Mock<IServiceManager>();



            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowersAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ReturnsAsync(pagedResponse);

            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };

            //Act
            var result = await controller.GetAllFollowersByUserName(testUserId, requestParameters);

            //Assert

            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var followers = okObjectResult.Value.Should().BeOfType<List<FollowerDto>>().Subject;

            controller.Response.Headers.Should().ContainKey("X-Pagination");

            var headerValue = controller.Response.Headers["X-Pagination"].ToString();
            headerValue.Should().Be(pagedResponse.MetaData.ToString());
        }




        [Fact]
        public async Task GetAllFollowersByUserName_WithPrivateUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowersAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserAccountIsPrivateBadRequestException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowersByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserAccountIsPrivateBadRequestException>();
        }




        [Fact]
        public async Task GetAllFollowingByUserName_WithInvalidUsername_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowingsAsync(It.IsAny<string>(), loggedInUsername, requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserInvalidValueBadRequestException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowingByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();
        }


        [Fact]
        public async Task GetAllFollowingByUserName_WithInvalidUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowingsAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserNotFoundException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowingByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }


        [Fact]
        public async Task GetAllFollowingByUserName_WithValidUser_ReturnFollowerDtos()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            string testUserId = Guid.NewGuid().ToString();
            FollowParameters requestParameters = new() { PageNumber = 1 };

            var pagedResponse = new PagedResponse<FollowingDto>(
                items: new List<FollowingDto>(),
                metaData: new MetaData()
                {
                    CurrentPage = requestParameters.PageNumber,
                    PageSize = requestParameters.PageSize,
                    TotalCount = 0,
                    TotalPage = 0
                });

            var mockServiceManager = new Mock<IServiceManager>();



            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowingsAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ReturnsAsync(pagedResponse);

            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };

            //Act
            var result = await controller.GetAllFollowingByUserName(testUserId, requestParameters);

            //Assert

            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var followers = okObjectResult.Value.Should().BeOfType<List<FollowingDto>>().Subject;

            controller.Response.Headers.Should().ContainKey("X-Pagination");

            var headerValue = controller.Response.Headers["X-Pagination"].ToString();
            headerValue.Should().Be(pagedResponse.MetaData.ToString());
        }




        [Fact]
        public async Task GetAllFollowingByUserName_WithPrivateUser_ThrowException()
        {
            //Arrange
            string loggedInUsername = "loggedInUsername";
            FollowParameters requestParameters = new() { PageNumber = 1 };
            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s
            .FollowsService
            .GetAllFollowingsAsync(It.IsAny<string>(), It.IsAny<string>(), requestParameters, It.IsAny<bool>()))
                .ThrowsAsync(new UserAccountIsPrivateBadRequestException());


            var controller = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername)
            };
            //Act
            Func<Task> act = async () => await controller.GetAllFollowingByUserName(It.IsAny<string>(), requestParameters);

            //Assert
            await act.Should().ThrowAsync<UserAccountIsPrivateBadRequestException>();
        }



        #endregion Follow



        #region File Upload


        //FileBadRequestException
        //General Exception
        //MinioGeneralBadRequestException
        //UserNotfound


        //[Fact]
        //public async Task UserUpdateProfileImage_With

        #endregion File Upload

    }
}
