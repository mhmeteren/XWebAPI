

using Entities.Models;
using FluentAssertions;
using Moq;
using Repositories.EFCore;
using XWebAPI.Tests.Fixtures;

namespace XWebAPI.Tests.Systems.Repositories
{
    public class UserRepositoryTests
    {

        [Fact]
        public async Task GetUserByUsername_WithValidAndExistentUsername_ReturnUser()
        {
            //Arrange
            string existentUsername = "Username-1";
            var dbContext = await UserFixtures.GetDatabaseContextWithUsersData();
            var repository = new UsersRepository(dbContext);


            //act
            var result = await repository.GetUserByUsernameAsync(existentUsername, It.IsAny<bool>());


            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Users>();
            result.UserName.Should().Be(existentUsername);
        }


        [Fact]
        public async Task GetUserByUsername_WithValidAndnonExistentUsername_ReturnDefault()
        {
            //Arrange
            string existentUsername = "Username-0";
            var dbContext = await UserFixtures.GetDatabaseContextWithUsersData();
            var repository = new UsersRepository(dbContext);

            //act
            var result = await repository.GetUserByUsernameAsync(existentUsername, It.IsAny<bool>());

            //Assert
            result.Should().BeNull();
        }



    }
}
