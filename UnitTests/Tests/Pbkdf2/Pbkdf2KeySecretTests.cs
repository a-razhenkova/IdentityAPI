using Application;
using Bogus;
using FluentAssertions;

namespace Pbkdf2Tests
{
    public class Pbkdf2KeySecretTests
    {
        [Fact]
        public void Create_WithRandomSize_ReturnSecret()
        {
            // Arrange
            int size = new Faker().Random.Int(16, 32);

            // Act
            byte[] secret = Pbkdf2KeySecret.Create(size);

            // Assert
            secret.Should().HaveCount(size);
        }

        [Fact]
        public void Create_WithZeroSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => Pbkdf2KeySecret.Create(0));

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void Create_WithNegativeSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => Pbkdf2KeySecret.Create(-1));

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void CreateToBase64_WithRandomSize_ReturnSecret()
        {
            // Arrange
            int size = new Faker().Random.Int(16, 32);

            // Act
            string secret = Pbkdf2KeySecret.CreateToBase64(size);

            // Assert
            secret.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateToBase64_WithZeroSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => Pbkdf2KeySecret.CreateToBase64(0));

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void CreateToBase64_WithNegativeSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => Pbkdf2KeySecret.CreateToBase64(-1));

            // Act
            func.Should().Throw();
        }
    }
}