using Application;
using FluentAssertions;

namespace UnitTests
{
    public class Pbkdf2KeyTests
    {
        private const int Iterations = 100_000;
        private const int HashLength = 128;
        private const int SaltLength = 16;

        [Theory]
        [InlineData("password")]
        public void Create_HashSameWordTwice_ReturnDifferentHashes(string word)
        {
            // Act
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);

            // Assert
            hash.Should().NotBe(newHash);
            salt.Should().NotBe(newSalt);
        }

        [Theory]
        [InlineData("password")]
        public void Recreate_SameWord_ReturnMatchingHashes(string word)
        {
            // Act
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(word, salt, Iterations, HashLength);

            // Assert
            hash.Should().Be(newHash);
            salt.Should().Be(newSalt);
        }

        [Theory]
        [InlineData("password")]
        public void IsValid_ValidHash_ReturnTrue(string word)
        {
            // Arrange
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, word, salt, Iterations, HashLength);

            // Assert
            result.Should().Be(true);
        }

        [Theory]
        [InlineData("password")]
        public void IsValid_InvalidHash_ReturnFalse(string word)
        {
            // Arrange
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, $"different_{word}", salt, Iterations, HashLength);

            // Assert
            result.Should().Be(false);
        }
    }
}