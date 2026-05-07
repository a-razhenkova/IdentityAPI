using Application;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using Tests.Mocks;

namespace UnitTests
{
    public class UserRefreshTokenTests
    {
        [Fact]
        public void RefreshToken_Create_ReturnTokenWithClaims()
        {
            // Arrange
            string publicId = Guid.NewGuid().ToString();
            string issuer = Guid.NewGuid().ToString();
            string audience = Guid.NewGuid().ToString();

            var securitySettings = SettingsMock.CreateBasicSecuritySettings(issuer: issuer, audience: audience);
            var user = UserMock.CreateBasicUser(publicId: publicId);

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);

            // Assert
            var cfg = new RefreshToken(refreshToken, securitySettings);

            JwtSecurityToken? jwt = cfg.Decode();
            jwt.Should().NotBeNull();

            jwt.Claims.Count().Should().Be(6);

            cfg.GetClaim(jwt, TokenClaim.Issuer).Should().Be(issuer);
            cfg.GetClaim(jwt, TokenClaim.Audience).Should().Be(audience);
            cfg.GetClaim(jwt, TokenClaim.UserPublicId).Should().Be(publicId);
        }

        [Fact]
        public async Task RefreshToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var user = UserMock.CreateBasicUser();

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task RefreshToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: Guid.NewGuid().ToString(),
                audience: securitySettings.TokenAudience, refreshKey: securitySettings.RefreshToken.Key, lifetime: securitySettings.RefreshToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task RefreshToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: securitySettings.TokenIssuer,
                audience: Guid.NewGuid().ToString(), refreshKey: securitySettings.RefreshToken.Key, lifetime: securitySettings.RefreshToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task RefreshToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(securitySettings.TokenIssuer,
                audience: securitySettings.TokenAudience, refreshKey: "Si8lk4k2%Y-UT0~S(pU7YEC56h{K6GXD", lifetime: securitySettings.RefreshToken.LifetimeInSeconds);
            var user = UserMock.CreateBasicUser();

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Signature validation failed.");
        }

        [Fact]
        public async Task ValidateRefreshToken_Expired_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMock.CreateBasicSecuritySettings(lifetime: 1);
            var user = UserMock.CreateBasicUser();

            // Act
            string refreshToken = new RefreshToken(securitySettings).Create(user);

            Thread.Sleep(1_000);

            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}