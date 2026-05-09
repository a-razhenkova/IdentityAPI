using Application;
using Bogus;
using FluentAssertions;

namespace Pbkdf2Tests
{
    public class Pbkdf2KeyTests
    {
        private const int Iterations = 100_000;
        private const int HashLength = 128;
        private const int SaltLength = 16;

        [Fact]
        public void Create_HashSameStringTwice_ReturnDifferentHashes()
        {
            // Arrange
            string str = new Faker().Random.String();

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Assert
            hash.Should().NotBe(newHash);
            salt.Should().NotBe(newSalt);
        }

        [Fact]
        public void RecreateString_ReturnMatchingHashes()
        {
            // Arrange
            string str = new Faker().Random.String();

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(str, salt, Iterations, HashLength);

            // Assert
            hash.Should().Be(newHash);
            salt.Should().Be(newSalt);
        }

        [Fact]
        public void IsValid_ValidHash_ReturnTrue()
        {
            // Arrange
            string str = new Faker().Random.String();
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, str, salt, Iterations, HashLength);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void IsValid_InvalidHash_ReturnFalse()
        {
            // Arrange
            string str = new Faker().Random.String();
            (string hash, string salt) = Pbkdf2Key.Create(str, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, $"different_{str}", salt, Iterations, HashLength);

            // Assert
            result.Should().Be(false);
        }
    }
}