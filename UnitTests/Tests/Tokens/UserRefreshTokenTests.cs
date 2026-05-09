using Application;
using Domain;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using UnitTests.Mocks;

namespace TokenTests
{
    public class UserRefreshTokenTests
    {
        [Theory]
        [InlineData(UserRoles.Administrator, UserStatuses.Active)]
        [InlineData(UserRoles.Customer, UserStatuses.Blocked)]
        public void RefreshToken_Create_ReturnTokenWithClaims(UserRoles userRole, UserStatuses userStatus)
        {
            // Arrange
            var securitySettings = new SecuritySettingsFaker().Generate();

            var user = new UserFaker().Generate();
            user.Role = userRole;
            user.Status.Value = userStatus;

            // Act
            var refreshToken = new RefreshToken(securitySettings);
            refreshToken.Create(user);

            // Assert
            JwtSecurityToken? jwt = refreshToken.Decode();
            jwt.Should().NotBeNull();
            jwt.Claims.Count().Should().Be(6);

            refreshToken.GetClaim(jwt, TokenClaim.Issuer).Should().Be(securitySettings.TokenIssuer);
            refreshToken.GetClaim(jwt, TokenClaim.Audience).Should().Be(securitySettings.TokenAudience);
            refreshToken.GetClaim(jwt, TokenClaim.UserPublicId).Should().Be(user.PublicId);
        }

        [Fact]
        public async Task RefreshToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var user = new UserFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().Generate();

            // Act
            var refreshToken = new RefreshToken(securitySettings);
            refreshToken.Create(user);

            TokenValidationResult tokenValidationResult = await refreshToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task RefreshToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentIssuer = securitySettingsFaker.SetNewTokenIssuer().Generate();

            // Act
            var refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentIssuer).ValidateAsync();

            // Assert
            securitySettings.TokenIssuer.Should().NotBe(securitySettingsWithDifferentIssuer.TokenIssuer);
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task RefreshToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentAudience = securitySettingsFaker.SetNewTokenAudience().Generate();

            // Act
            var refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentAudience).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task RefreshToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentKey = securitySettingsFaker.SetNewRefreshTokenKey().Generate();

            // Act
            var refreshToken = new RefreshToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new RefreshToken(refreshToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Signature validation failed.");
        }

        [Theory]
        [InlineData(1)]
        public async Task ValidateRefreshToken_Expired_ReturnIsValidFalse(int lifetimeInSeconds)
        {
            // Arrange
            var user = new UserFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().SetRefreshTokenLifetime(lifetimeInSeconds).Generate();

            // Act
            var refreshToken = new RefreshToken(securitySettings);
            refreshToken.Create(user);

            Thread.Sleep(lifetimeInSeconds * 1_000);

            TokenValidationResult tokenValidationResult = await refreshToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}