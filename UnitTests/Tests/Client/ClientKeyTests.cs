using Application;
using Bogus;
using FluentAssertions;

namespace ClientTests
{
    public class ClientKeyTests
    {
        [Fact]
        public void Create_ReturnKey()
        {
            // Act
            string key = ClientKey.Create();

            // Assert
            key.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(10)]
        public void Create_MultipleTimes_ReturnUniqueKeys(int createCount)
        {
            // Arrange
            var keys = new List<string>();

            // Act
            for (int index = 0; index < createCount; index++)
                keys.Add(ClientKey.Create());

            // Assert
            keys.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void IsValid_CreateValidKey_ReturnTrue()
        {
            // Arrange
            string key = ClientKey.Create();

            // Act
            bool result = ClientKey.IsValid(key);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_CreateInvalidKey_ReturnFalse()
        {
            // Arrange
            string key = new Faker().Random.String(4);

            // Act
            bool result = ClientKey.IsValid(key);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_EmptyKey_ReturnFalse()
        {
            // Act
            bool result = ClientKey.IsValid(string.Empty);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_NullKey_ThrowException()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var func = FluentActions.Invoking(() => ClientKey.IsValid(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act
            func.Should().Throw();
        }
    }
}