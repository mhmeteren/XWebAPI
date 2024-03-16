using Entities.DataTransferObjects.User;
using Entities.Exceptions.BaseUser;
using Entities.Exceptions.User;
using Entities.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Repositories.Contracts;
using Services;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services
{
    public class UserServiceTests
    {

        #region GetUserByUserName

        [Fact]
        public async Task GetUserByUsername_WithInvalidUsernameLenght_ThrowException()
        {
            //Arrange
            string invalidUsername = "invalidUsername";
            var mockRepository = new Mock<IRepositoryManager>();
            var mockFileUploadServide = new Mock<IFileUploadService>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mockIdentityUserManager = UserFixtures.MockUserManager<BaseUser>();

            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(false);


            var service = new UsersManager(mockRepository.Object,
                SystemFixtures.GetAPIsMapper(),
                mockIdentityUserManager.Object,
                mockFileUploadServide.Object,
                mockValidatorservice.Object);


            //Act
            Func<Task> act = async () => await service.GetUserProfileByUsernameAsync(invalidUsername);


            //Assert
            await act.Should().ThrowAsync<UserInvalidValueBadRequestException>();
        }


        [Fact]
        public async Task GetUserByUsername_WithNonExistentUsername_ThrowUserNotFoundException()
        {
            //Arrange
            string nonExistentUsername = "testUsername";
            var mockRepository = new Mock<IRepositoryManager>();
            var mockFileUploadServide = new Mock<IFileUploadService>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mockIdentityUserManager = UserFixtures.MockUserManager<BaseUser>();


            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(true);

            mockIdentityUserManager.Setup(svc => svc.FindByNameAsync(nonExistentUsername))
                .ReturnsAsync(() => null);

            var service = new UsersManager(
                mockRepository.Object,
                SystemFixtures.GetAPIsMapper(),
                mockIdentityUserManager.Object,
                mockFileUploadServide.Object,
                mockValidatorservice.Object);

            //Act
            Func<Task> act = async () => await service.GetUserProfileByUsernameAsync(nonExistentUsername);

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();

        }


        [Fact]
        public async Task GetUserByUsername_WithValidAndExistentUsername_ReturnUserDto()
        {
            //Arrange
            string existentUsername = "testUsername";
            Users user = new()
            {
                FullName = "TestFullName",
                UserName = existentUsername,
                ProfileImageUrl = "/image",
                BackgroundImageUrl = "/image",
                About = "TestAbout"
            };



            var mockRepository = new Mock<IRepositoryManager>();
            var mockFileUploadServide = new Mock<IFileUploadService>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mockIdentityUserManager = UserFixtures.MockUserManager<BaseUser>();



            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(true);

            mockIdentityUserManager.Setup(svc => svc.FindByNameAsync(existentUsername))
                .ReturnsAsync(user);

            var service = new UsersManager(
                mockRepository.Object,
                SystemFixtures.GetAPIsMapper(),
                mockIdentityUserManager.Object,
                mockFileUploadServide.Object,
                mockValidatorservice.Object);

            //Act
            var result = await service.GetUserProfileByUsernameAsync(existentUsername);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UserProfileDto>();
            result.UserName.Should().Be(existentUsername);
        }


        #endregion

        [Fact]
        public async Task RegisterUserAsync_WithValidUserDto_ReturnIdentityResult()
        {
            // Arrange
            var mockRepositoryManager = new Mock<IRepositoryManager>();
            var mockFileUploadServide = new Mock<IFileUploadService>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mockMapper = SystemFixtures.GetAPIsMapper();
            var mockBaseUserManager = UserFixtures.MockUserManager<BaseUser>();

            var userDto = new UserDtoForRegister
            (
                FullName: "TestFullName",
                Email: "test@test.com",
                UserName: "Testusername",
                Birthday: DateTime.Now.AddYears(-20),
                Password: "ValIdP@ssw0rd!123"
            );

            var successResult = IdentityResult.Success;
            mockBaseUserManager.Setup(m => m.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
                               .ReturnsAsync(successResult);

            mockBaseUserManager.Setup(m => m.AddToRolesAsync(It.IsAny<Users>(), new List<string>() { "USER" }))
                   .ReturnsAsync(successResult);


            var userService = new UsersManager(
                mockRepositoryManager.Object,
                mockMapper,
                mockBaseUserManager.Object,
                mockFileUploadServide.Object,
                mockValidatorservice.Object);

            // Act
            var result = await userService.ReqisterUserAsync(userDto);

            // Assert
            result.Should().Be(successResult);

        }



        [Fact]
        public async Task UpdateProfile_WithValidUserDto_ReturnSucceeded()
        {
            //Arrenge

            var validUsername = "validUsername";
            Users user = new()
            {
                FullName = "TestFullName",
                UserName = validUsername,
                ProfileImageUrl = "/image",
                BackgroundImageUrl = "/image",
                About = "TestAbout",
                Birthday = DateTime.Now.AddYears(-20),
                Gender = "Male",
                Location = "localhost"
            };

            var userDto = new UserDtoForAccountUpdate
            (
                FullName: "test",
                About: "test",
                Birthday: DateTime.Now.AddYears(-20),
                Gender: "Male",
                Location: "localhost"
            );


            var repositoryManager = new Mock<IRepositoryManager>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var baseUserManager = UserFixtures.MockUserManager<BaseUser>();
            var fileUploadService = new Mock<IFileUploadService>();



            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(true);

            baseUserManager.Setup(m => m.FindByNameAsync(validUsername))
                .ReturnsAsync(user);


            baseUserManager.Setup(m => m.UpdateAsync(It.IsAny<Users>()))
                .ReturnsAsync(IdentityResult.Success);


            var userManager = new UsersManager(repositoryManager.Object,
                mapper,
                baseUserManager.Object,
                fileUploadService.Object,
                mockValidatorservice.Object);


            //Act
            var result = await userManager.UpdateProfile(validUsername, userDto);

            //Assert
            result.Should().Be(IdentityResult.Success);

        }


        [Fact]
        public async Task UpdateProfile_WithInvalidUserDto_ReturnFailed()
        {
            //Arrenge

            var validUsername = "validUsername";
            Users user = new()
            {
                FullName = "TestFullName",
                UserName = validUsername,
                ProfileImageUrl = "/image",
                BackgroundImageUrl = "/image",
                About = "TestAbout",
                Birthday = DateTime.Now.AddYears(-20),    
                Gender = "Male",
                Location = "localhost"
            };


            var userDto = new UserDtoForAccountUpdate
             (
                 FullName: "test",
                 About: "test",
                 Birthday: DateTime.Now.AddYears(-20),
                 Gender: "Male",
                 Location: "localhost"
             );



            var repositoryManager = new Mock<IRepositoryManager>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var baseUserManager = UserFixtures.MockUserManager<BaseUser>();
            var fileUploadService = new Mock<IFileUploadService>();

            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(true);


            baseUserManager.Setup(m => m.FindByNameAsync(validUsername))
                .ReturnsAsync(user);

            var identityFailed = IdentityResult
                    .Failed(new IdentityError() { Code = "test", Description = "test error message" });

            baseUserManager.Setup(m => m.UpdateAsync(It.IsAny<Users>()))
                .ReturnsAsync(identityFailed);


            var userManager = new UsersManager(repositoryManager.Object,
                mapper,
                baseUserManager.Object,
                fileUploadService.Object,
                mockValidatorservice.Object);


            //Act
            var result = await userManager.UpdateProfile(validUsername, userDto);

            //Assert
            result.Should().Be(identityFailed);

        }



        [Fact]
        public async Task UpdateProfile_WithValidOrInvalidUserDto_ThrowUserNotFoundException()
        {
            //Arrenge

            var nonExistentUsername = "validUsername";

            var repositoryManager = new Mock<IRepositoryManager>();
            var mockValidatorservice = new Mock<IValidatorService>();
            var mapper = SystemFixtures.GetAPIsMapper();
            var baseUserManager = UserFixtures.MockUserManager<BaseUser>();
            var fileUploadService = new Mock<IFileUploadService>();

            mockValidatorservice.Setup(i => i.IsValidUsername(It.IsAny<string>()))
                .Returns(true);

            baseUserManager.Setup(svc => svc.FindByNameAsync(nonExistentUsername))
                .ReturnsAsync(() => null);


            var userManager = new UsersManager(
                repositoryManager.Object,
                mapper,
                baseUserManager.Object,
                fileUploadService.Object,
                mockValidatorservice.Object);


            //Act
            Func<Task> act = async () => await userManager.UpdateProfile(nonExistentUsername, It.IsAny<UserDtoForAccountUpdate>());

            //Assert
            await act.Should().ThrowAsync<UserNotFoundException>();

        }




    }
}
