using Application;
using FluentAssertions;

namespace UserTests
{
    public class UserOtpKeyTests
    {
        [Fact]
        public void Create_ReturnKey()
        {
            // Act
            string key = UserOtpKey.Create();

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
                keys.Add(UserOtpKey.Create());

            // Assert
            keys.Should().OnlyHaveUniqueItems();
        }
    }
}