using Application;
using Domain;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using Tests.Mocks;

namespace UnitTests
{
    public class UserAccessTokenTests
    {
        [Theory]
        [InlineData("ivan.ivanov", UserRoles.Administrator, UserStatuses.Active)]
        [InlineData("petar.petrov", UserRoles.Customer, UserStatuses.Blocked)]
        public void AccessToken_Create_ReturnTokenWithClaims(string username, UserRoles userRole, UserStatuses userStatus)
        {
            // Arrange
            string publicId = Guid.NewGuid().ToString();
            string issuer = Guid.NewGuid().ToString();
            string audience = Guid.NewGuid().ToString();

            var securitySettings = SettingsMock.CreateBasicSecuritySettings(issuer: issuer, audience: audience);
            var user = UserMock.CreateBasicUser(publicId: publicId, username: username, role: userRole, status: userStatus);

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);

            // Assert
            var cfg = new AccessToken(accessToken, securitySettings);

            JwtSecurityToken? jwt = cfg.Decode();
            jwt.Should().NotBeNull();

            jwt.Claims.Count().Should().Be(9);

            cfg.GetClaim(jwt, TokenClaim.Issuer).Should().Be(issuer);
            cfg.GetClaim(jwt, TokenClaim.Audience).Should().Be(audience);
            cfg.GetClaim(jwt, TokenClaim.UserPublicId).Should().Be(publicId);
            cfg.GetClaim(jwt, TokenClaim.Username).Should().Be(username);
            cfg.GetClaim(jwt, TokenClaim.UserRole).Should().Be(userRole.ToString());
            cfg.GetClaim(jwt, TokenClaim.UserStatus).Should().Be(userStatus.ToString());
        }

        [Fact]
        public async Task AccessToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var user = UserMock.CreateBasicUser();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task AccessToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: Guid.NewGuid().ToString(),
                audience: securitySettings.TokenAudience, accessKey: securitySettings.AccessToken.Key, lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: securitySettings.TokenIssuer,
                audience: Guid.NewGuid().ToString(), accessKey: securitySettings.AccessToken.Key, lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: securitySettings.TokenIssuer,
                audience: securitySettings.TokenAudience, accessKey: "VQJAz9-2cJu4?4|baop4#&E4sBtO0F/f", lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Signature validation failed.");
        }

        [Fact]
        public async Task ValidateAccessToken_Expired_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings(lifetime: 1);
            var user = UserMock.CreateBasicUser();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(user);

            Thread.Sleep(1_000);

            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}