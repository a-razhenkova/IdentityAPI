using Bogus;
using FluentAssertions;
using Shared;

namespace RandomKeyTests
{
    public class RandomKeyTests
    {
        [Fact]
        public void Create_WithRandomSize_ReturnSecret()
        {
            // Arrange
            int size = new Faker().Random.Int(16, 32);

            // Act
            byte[] secret = new RandomKey(size).Create();

            // Assert
            secret.Should().HaveCount(size);
        }

        [Theory]
        [InlineData(10)]
        public void Create_MultipleTimesWithSameSize_ReturnUniqueSecrets(int createCount)
        {
            // Arrange
            var secrets = new List<byte[]>();
            int size = new Faker().Random.Int(16, 32);

            // Act
            for (int index = 0; index < createCount; index++)
                secrets.Add(new RandomKey(size).Create());

            // Assert
            secrets.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void Create_WithZeroSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => new RandomKey(0).Create());

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void Create_WithNegativeSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => new RandomKey(-1).Create());

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void CreateToBase64_WithRandomSize_ReturnSecret()
        {
            // Arrange
            int size = new Faker().Random.Int(16, 32);

            // Act
            string secret = new RandomKey(size).CreateToBase64();

            // Assert
            secret.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(10)]
        public void CreateToBase64_MultipleTimesWithSameSize_ReturnUniqueSecrets(int createCount)
        {
            // Arrange
            var secrets = new List<string>();
            int size = new Faker().Random.Int(16, 32);

            // Act
            for (int index = 0; index < createCount; index++)
                secrets.Add(new RandomKey(size).CreateToBase64());

            // Assert
            secrets.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void CreateToBase64_WithZeroSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => new RandomKey(0).CreateToBase64());

            // Act
            func.Should().Throw();
        }

        [Fact]
        public void CreateToBase64_WithNegativeSize_ThrowException()
        {
            // Arrange
            var func = FluentActions.Invoking(() => new RandomKey(-1).CreateToBase64());

            // Act
            func.Should().Throw();
        }
    }
}