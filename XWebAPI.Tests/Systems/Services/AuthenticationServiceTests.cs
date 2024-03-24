
using Entities.DataTransferObjects.BaseUser;
using Entities.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using Services.Contracts;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Services
{
    public class AuthenticationServiceTests
    {

        [Fact]
        public async Task UserValidation_WithValidUser_ReturnTrue()
        {

            //Arrange
            BaseUser mockUser = new()
            {
                UserName = "Test"
            };

            BaseUserDtoForLogin mockUserLogin = new
            (
                Username : "Test",
                Password : "Test"
            );


            var mockConfigService = new Mock<IConfiguration>();
            var mockCacheService = new Mock<ICacheService>();
            var mockIdentityUserManager = UserFixtures.MockUserManager<BaseUser>();



            var mockConfigSection = AuthenticationFixtures.GetMockJWTSettings();
            mockConfigService.Setup(c => c.GetSection("JwtSettings"))
                .Returns(mockConfigSection.Object);




            mockCacheService.Setup(c => c.SetCachedData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);




            mockIdentityUserManager.Setup(s => s.FindByNameAsync(mockUserLogin.Username))
                .ReturnsAsync(mockUser);

            mockIdentityUserManager.Setup(s => s.CheckPasswordAsync(mockUser, mockUserLogin.Password))
                .ReturnsAsync(true);

            mockIdentityUserManager.Setup(s => s.IsEmailConfirmedAsync(mockUser))
                .ReturnsAsync(true);


            mockIdentityUserManager.Setup(s => s.GetRolesAsync(It.IsAny<BaseUser>()))
                .ReturnsAsync(["Test"]);




            var service = new AuthenticationManager(mockCacheService.Object, mockIdentityUserManager.Object, mockConfigService.Object);

            //Act

            var validateUserResult = await service.ValidateUser(mockUserLogin);
            var createTokenResult = await service.CreateToken(true);


            //Assert
            validateUserResult.Should().BeTrue();
            createTokenResult.Should().BeOfType<TokenDto>().Subject.Should().NotBe(default);
            
        }


        [Fact]
        public async Task UserValidation_WithNotConfirmedEmail_ReturnFalse()
        {

            //Arrange
            BaseUser mockUser = new()
            {
                UserName = "Test"
            };

            BaseUserDtoForLogin mockUserLogin = new
            (
                Username : "Test",
                Password : "Test"
            );


            var mockConfigService = new Mock<IConfiguration>();
            var mockCacheService = new Mock<ICacheService>();
            var mockIdentityUserManager = UserFixtures.MockUserManager<BaseUser>();




            mockIdentityUserManager.Setup(s => s.FindByNameAsync(mockUserLogin.Username))
                .ReturnsAsync(mockUser);

            mockIdentityUserManager.Setup(s => s.CheckPasswordAsync(mockUser, mockUserLogin.Password))
                .ReturnsAsync(true);

            mockIdentityUserManager.Setup(s => s.IsEmailConfirmedAsync(mockUser))
                .ReturnsAsync(false);


            var service = new AuthenticationManager(mockCacheService.Object, mockIdentityUserManager.Object, mockConfigService.Object);

            //Act

            var validateUserResult = await service.ValidateUser(mockUserLogin);
            

            //Assert
            validateUserResult.Should().BeFalse();
        }
    
    
    
    }
}
