using Application;
using Bogus;
using FluentAssertions;

namespace Pbkdf2KeyTests
{
    public class Pbkdf2KeyTests
    {
        private const int Iterations = 100_000;
        private const int HashLength = 128;
        private const int SaltLength = 16;

        [Theory]
        [InlineData(32)]
        public void Create_ReturnHash(int strLength)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Assert
            hash.Should().NotBeNullOrWhiteSpace();
            salt.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(32, 10)]
        public void Create_MultipleTimes_ReturnUniqueHashes(int strLength, int createCount)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);
            var hashes = new List<string>();
            var salts = new List<string>();

            // Act
            for (int index = 0; index < createCount; index++)
            {
                (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);
                hashes.Add(hash);
                salts.Add(salt);
            }

            // Assert
            hashes.Should().OnlyHaveUniqueItems();
            salts.Should().OnlyHaveUniqueItems();
        }

        [Theory]
        [InlineData(32)]
        public void Recreate_ReturnHash(int strLength)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(str, salt, Iterations, HashLength);

            // Assert
            newHash.Should().NotBeNullOrWhiteSpace();
            newSalt.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(32)]
        public void Recreate_MultipleTimes_ReturnSameHash(int strLength)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(str, salt, Iterations, HashLength);

            // Assert
            hash.Should().Be(newHash);
            salt.Should().Be(newSalt);
        }

        [Theory]
        [InlineData(32)]
        public void IsValid_ValidHash_ReturnTrue(int strLength)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, str, salt, Iterations, HashLength);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(32)]
        public void IsValid_InvalidHash_ReturnFalse(int strLength)
        {
            // Arrange
            string str = new Faker().Random.String(strLength);
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, $"different_{str}", salt, Iterations, HashLength);

            // Assert
            result.Should().BeFalse();
        }
    }
}