using Application;
using Bogus;
using FluentAssertions;

namespace UserTests
{
    public class UserPasswordTests
    {
        [Theory]
        [InlineData(32)]
        public void Create_ReturnPassword(int passwordLength)
        {
            // Arrange
            string password = new Faker().Random.String(passwordLength);

            // Act
            var securePassword = UserPassword.Create(password);

            // Assert
            securePassword.Value.Should().NotBeNullOrWhiteSpace();
            securePassword.Secret.Should().NotBeNullOrWhiteSpace();
            securePassword.LastChangedTimestamp.Should().BeSameDateAs(DateTime.UtcNow);
        }

        [Theory]
        [InlineData(32, 10)]
        public void Create_MultipleTimes_ReturnUniquePasswords(int passwordLength, int createCount)
        {
            // Arrange
            string password = new Faker().Random.String();
            var hashes = new List<string>();
            var secrets = new List<string>();

            // Act
            for (int index = 0; index < createCount; index++)
            {
                var securePassword = UserPassword.Create(password);
                hashes.Add(securePassword.Value);
                secrets.Add(securePassword.Secret);
            }

            // Assert
            hashes.Should().OnlyHaveUniqueItems();
            secrets.Should().OnlyHaveUniqueItems();
        }

        [Theory]
        [InlineData(32)]
        public void IsMatch_ValidHash_ReturnTrue(int passwordLength)
        {
            // Arrange
            string password = new Faker().Random.String(passwordLength);
            var securePassword = UserPassword.Create(password);

            // Act
            bool result = UserPassword.IsMatch(securePassword, password);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(32)]
        public void IsValid_InvalidHash_ReturnFalse(int passwordLength)
        {
            // Arrange
            string password = new Faker().Random.String(passwordLength);
            var securePassword = UserPassword.Create(password);

            // Act
            bool result = UserPassword.IsMatch(securePassword, $"different_{password}");

            // Assert
            result.Should().BeFalse();
        }
    }
}