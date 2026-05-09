using Application;
using FluentAssertions;

namespace UserTests
{
    public class UserOtpTests
    {
        [Fact]
        public void Create_ReturnOtp()
        {
            // Act
            string otp = UserOtp.Create();

            // Assert
            otp.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(10)]
        public void Create_MultipleTimes_ReturnUniqueOtps(int createCount)
        {
            // Arrange
            var otps = new List<string>();

            // Act
            for (int index = 0; index < createCount; index++)
                otps.Add(UserOtp.Create());

            // Assert
            otps.Should().OnlyHaveUniqueItems();
        }
    }
}