using Application;
using Bogus;
using FluentAssertions;

namespace ClientTests
{
    public class ClientSecretTests
    {
        [Fact]
        public void Create_ReturnSecret()
        {
            // Act
            string secret = ClientSecret.Create();

            // Assert
            secret.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(10)]
        public void Create_MultipleTimes_ReturnUniqueSecrets(int createCount)
        {
            // Arrange
            var secrets = new List<string>();

            // Act
            for (int index = 0; index < createCount; index++)
                secrets.Add(ClientSecret.Create());

            // Assert
            secrets.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void IsValid_CreateValidKey_ReturnTrue()
        {
            // Arrange
            string secret = ClientSecret.Create();

            // Act
            bool result = ClientSecret.IsValid(secret);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_CreateInvalidKey_ReturnFalse()
        {
            // Arrange
            string secret = new Faker().Random.String(4);

            // Act
            bool result = ClientSecret.IsValid(secret);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_EmptySecret_ReturnFalse()
        {
            // Act
            bool result = ClientSecret.IsValid(string.Empty);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_NullSecret_ThrowException()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var func = FluentActions.Invoking(() => ClientSecret.IsValid(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act
            func.Should().Throw();
        }
    }
}