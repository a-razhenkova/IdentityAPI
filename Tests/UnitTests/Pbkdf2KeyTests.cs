using Application;
using FluentAssertions;
using Xunit.Priority;

namespace UnitTests
{
    public class Pbkdf2KeyTests
    {
        [Priority(1)]
        [Theory]
        [InlineData("m4A0?Edis66a", 100_000, 128, 16)]
        public void CreateHash_RecreateSameHash(string password, int interactions, int hashLength, int saltLength)
        {
            // Arrange
            (string hash, string salt) = Pbkdf2Key.Create(password, interactions, hashLength, saltLength);

            // Act
            (string newHash, string newSalt) = Pbkdf2Key.Recreate(password, salt, interactions, hashLength);

            // Assert
            hash.Should().Be(newHash);
            salt.Should().Be(newSalt);
        }

        [Priority(2)]
        [Theory]
        [InlineData("m4A0?Edis66a", 100_000, 128, 16)]
        public void HashUniquenessTest(string password, int interactions, int hashLength, int saltLength)
        {
            // Arrange
            (string hash, string salt) = Pbkdf2Key.Create(password, interactions, hashLength, saltLength);

            // Act
            (string newHash, string newSalt) = Pbkdf2Key.Create(password, interactions, hashLength, saltLength);

            // Assert
            hash.Should().NotBe(newHash);
            salt.Should().NotBe(newSalt);
        }
    }
}