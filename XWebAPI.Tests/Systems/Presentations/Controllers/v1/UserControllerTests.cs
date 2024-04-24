using Services.Contracts;
using Moq;
using Entities.Exceptions.User;
using Presentation.Controllers.v1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Entities.DataTransferObjects.User;
using Microsoft.AspNetCore.Identity;
using Entities.Exceptions.BaseUser;
using XWebAPI.Tests.Fixtures;
using Entities.DataTransferObjects.Follower;
using Entities.RequestFeatures;
using Presentation.Validators.User;


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
            UserProfileDto userDto = new(
                Id:"Id",
                FullName: "FullName",
                UserName: "UserName",
                ProfileImageUrl : "/image",
                BackgroundImageUrl: "/image",
                About: "test",
                Location: "",
                IsPrivateAccount: true,
                IsVerifiedAccount: false
            );

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
        public async Task RegisterUser_WithInValidUserObject_ReturnBadRequestByFluentValidation()
        {

            //Arrange
            var userDto = new UserDtoForRegister
            (
                FullName: "u",
                Email: "t",
                UserName: "u",
                Birthday: DateTime.Now.AddYears(-5),
                Password: "p"
            );

            var mockService = new Mock<IServiceManager>();
            var validator = new UserRegisterValidator();
            var controller = new UserController(mockService.Object);

            //Act
            var result = await controller.RegisterUser(userDto, validator);


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            controller.ModelState.Keys.Should().NotContain("Password");

            var expectedErrorKeys = new List<string> { "FullName", "Email", "UserName", "Birthday" };
            expectedErrorKeys.ForEach(i => controller.ModelState.Keys.Should().Contain(i));
        }



        [Fact]
        public async Task RegisterUser_WithInValidUserObject_ReturnBadRequestByIdentityValidation()
        {

            //Arrange
            var userDto = new UserDtoForRegister
            (
                FullName: "ValidFullName",
                Email: "validemail@x.com",
                UserName: "validusername",
                Birthday: DateTime.Now.AddYears(-20),
                Password: "p"
            );

            var failedPasswordPolicyResult = IdentityResult.Failed(
                new IdentityError { Code = "Password", Description = "Password does not meet the policy requirements" });

            var mockService = new Mock<IServiceManager>();
            mockService.Setup(s => s.UserService.ReqisterUserAsync(userDto))
                .ReturnsAsync(failedPasswordPolicyResult);


            var validator = new UserRegisterValidator();
            var controller = new UserController(mockService.Object);

            //Act
            var result = await controller.RegisterUser(userDto, validator);


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            controller.ModelState.Keys.Should().Contain("Password");

            var expectedErrorKeys = new List<string> { "FullName", "Email", "UserName", "Birthday" };
            expectedErrorKeys.ForEach(i => controller.ModelState.Keys.Should().NotContain(i));
        }






        [Fact]
        public async Task RegisterUser_WithValidUserObject_ReturnHttpCreated()
        {

            //Arrange

            var userDto = new UserDtoForRegister
            (
                FullName: "TestFullName",
                Email: "test@test.com",
                UserName: "Testusername",
                Birthday: DateTime.Now.AddYears(-20),
                Password: "ValIdP@ssw0rd!123"
            );

            var validator = new UserRegisterValidator();

            var mockService = new Mock<IServiceManager>();
            mockService.Setup(s => s.UserService.ReqisterUserAsync(userDto))
                .ReturnsAsync(IdentityResult.Success);


            var controller = new UserController(mockService.Object);

            //Act   
            var result = await controller.RegisterUser(userDto, validator);


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
            var loggedInUsername = "testUser";

            var userDto = new UserDtoForAccountUpdate
            (
                FullName: "ValidFullName",
                About: "ValidAbout",
                Birthday: DateTime.Now.AddYears(-20),
                Gender: "Male",
                Location: "localhost"
            );

            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(loggedInUsername, userDto))
                    .ReturnsAsync(IdentityResult.Success);

            var validator = new UserAccountUpdateValidator();

            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername),
            };



            //Act
            var result = await userController.UserUpdateProfile(userDto, validator);


            //Assert
            var createdResult = result.Should().BeOfType<OkObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_Return400BadRequestByFluentValidation()
        {
            //Arrange
            var userDto = new UserDtoForAccountUpdate
            (
                FullName: "",
                About: "",
                Birthday: DateTime.Now.AddYears(-5),
                Gender: "InvalidGender",
                Location: ""
            );

            var mockServiceManager = new Mock<IServiceManager>();
            var validator = new UserAccountUpdateValidator();
            var userController = new UserController(mockServiceManager.Object);


            //Act
            var result = await userController.UserUpdateProfile(userDto, validator);


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            var expectedErrorKeys = new List<string> { "FullName", "About", "Birthday", "Gender", "Location" };
            expectedErrorKeys.ForEach(i => userController.ModelState.Keys.Should().Contain(i));
        }

        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_Return400BadRequestByIdentityValidation()
        {

            //Arrange
            var loggedInUsername = "testUser";

            var userDto = new UserDtoForAccountUpdate
            (
                FullName: "ValidFullName",
                About: "ValidAbout",
                Birthday: DateTime.Now.AddYears(-20),
                Gender: "Male",
                Location: "localhost"
            );

            var mockServiceManager = new Mock<IServiceManager>();

            var mockIdentityFailed = IdentityResult.Failed(
                    new IdentityError { Code = "CustomErrorKey", Description = "CustomErrorMesssage" });

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(loggedInUsername, It.IsAny<UserDtoForAccountUpdate>()))
                    .ReturnsAsync(mockIdentityFailed);

            var validator = new UserAccountUpdateValidator();

            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = SystemFixtures.ControllerContextConfigure(loggedInUsername),
            };


            //Act
            var result = await userController.UserUpdateProfile(userDto, validator);


            //Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            userController.ModelState.Keys.Should().Contain(mockIdentityFailed.Errors.First().Code);
        }


        [Fact]
        public async Task UserUpdateProfile_WithValidUserDto_ThrowUserNotFoundException()
        {

            //Arrange
            var loggedInUsername = "testUser";
            var userDto = new UserDtoForAccountUpdate
            (
                FullName: "ValidFullName",
                About: "ValidAbout",
                Birthday: DateTime.Now.AddYears(-20),
                Gender: "Male",
                Location: "localhost"
            );


            var mockServiceManager = new Mock<IServiceManager>();

            mockServiceManager.Setup(s => s.UserService.UpdateProfile(loggedInUsername, It.IsAny<UserDtoForAccountUpdate>()))
                    .ThrowsAsync(new UserNotFoundException());

            var validator = new UserAccountUpdateValidator();

            var userController = new UserController(mockServiceManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = UserFixtures.GetMockControllerContext(loggedInUsername),
                }
            };


            //Act
            Func<Task> act = async () => await userController.UserUpdateProfile(userDto, validator);


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
