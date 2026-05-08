using Application;
using FluentAssertions;

namespace UnitTests
{
    public class Pbkdf2KeyTests
    {
        private const int Iterations = 100_000;
        private const int HashLength = 128;
        private const int SaltLength = 16;

        [Fact]
        public void Create_HashSameWordTwice_ReturnDifferentHashes()
        {
            // Arrange
            const string word = "test";

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);

            // Assert
            hash.Should().NotBe(newHash);
            salt.Should().NotBe(newSalt);
        }

        [Fact]
        public void Recreate_SameWord_ReturnMatchingHashes()
        {
            // Arrange
            const string word = "word";

            // Act
            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(word, salt, Iterations, HashLength);

            // Assert
            hash.Should().Be(newHash);
            salt.Should().Be(newSalt);
        }

        [Fact]
        public void IsValid_ValidHash_ReturnTrue()
        {
            // Arrange
            const string word = "word";

            (string hash, string salt) = Pbkdf2Key.Create(word, Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, word, salt, Iterations, HashLength);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void IsValid_InvalidHash_ReturnFalse()
        {
            // Arrange
            (string hash, string salt) = Pbkdf2Key.Create("word", Iterations, HashLength, SaltLength);

            // Act
            bool result = Pbkdf2Key.IsValid(hash, "different_word", salt, Iterations, HashLength);

            // Assert
            result.Should().Be(false);
        }
    }
}