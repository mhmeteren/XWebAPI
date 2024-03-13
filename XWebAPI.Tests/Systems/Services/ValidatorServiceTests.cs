
using FluentAssertions;
using Services;

namespace XWebAPI.Tests.Systems.Services
{
    public class ValidatorServiceTests
    {
        [Fact]
        public void IsValidUsername_WithValidUsername_ReturnTrue()
        {
            //Arrange
            string validUsername = "validUsername";
            var manager = new ValidatorManager();   

            //Act
            var result = manager.IsValidUsername(validUsername); 


            //Assert
            result.Should().BeTrue();
        }


        [Fact]
        public void IsValidUsername_WithNullUsername_ReturnFalse()
        {
            //Arrange
            string inValidUsername = null;
            var manager = new ValidatorManager();

            //Act
            var result = manager.IsValidUsername(inValidUsername);


            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUsername_WithEmptyUsername_ReturnFalse()
        {
            //Arrange
            string inValidUsername = "";
            var manager = new ValidatorManager();

            //Act
            var result = manager.IsValidUsername(inValidUsername);


            //Assert
            result.Should().BeFalse();
        }


        [Fact]
        public void IsValidUsername_WithSpaceCharactersUsername_ReturnFalse()
        {
            //Arrange
            string inValidUsername = " inValid Username ";
            var manager = new ValidatorManager();

            //Act
            var result = manager.IsValidUsername(inValidUsername);


            //Assert
            result.Should().BeFalse();
        }


        [Fact]
        public void IsValidUsername_WithShortUsername_ReturnFalse()
        {
            //Arrange
            string inValidUsername = "a";
            var manager = new ValidatorManager();

            //Act
            var result = manager.IsValidUsername(inValidUsername);


            //Assert
            result.Should().BeFalse();
        }


        [Fact]
        public void IsValidUsername_WithLongUsername_ReturnFalse()
        {
            //Arrange
            string inValidUsername = new('a', 101);
            var manager = new ValidatorManager();

            //Act
            var result = manager.IsValidUsername(inValidUsername);


            //Assert
            result.Should().BeFalse();
        }
    }
}
