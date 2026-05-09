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
        [InlineData(UserRoles.Administrator, UserStatuses.Active)]
        [InlineData(UserRoles.Customer, UserStatuses.Blocked)]
        public void AccessToken_Create_ReturnTokenWithClaims(UserRoles userRole, UserStatuses userStatus)
        {
            // Arrange
            var securitySettings = new SecuritySettingsFaker().Generate();

            var user = new UserFaker().Generate();
            user.Role = userRole;
            user.Status.Value = userStatus;

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(user);

            // Assert
            JwtSecurityToken? jwt = accessToken.Decode();
            jwt.Should().NotBeNull();
            jwt.Claims.Count().Should().Be(9);

            accessToken.GetClaim(jwt, TokenClaim.Issuer).Should().Be(securitySettings.TokenIssuer);
            accessToken.GetClaim(jwt, TokenClaim.Audience).Should().Be(securitySettings.TokenAudience);
            accessToken.GetClaim(jwt, TokenClaim.UserPublicId).Should().Be(user.PublicId);
            accessToken.GetClaim(jwt, TokenClaim.Username).Should().Be(user.Username);
            accessToken.GetClaim(jwt, TokenClaim.UserRole).Should().Be(userRole.ToString());
            accessToken.GetClaim(jwt, TokenClaim.UserStatus).Should().Be(userStatus.ToString());
        }

        [Fact]
        public async Task AccessToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var user = new UserFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(user);

            TokenValidationResult tokenValidationResult = await accessToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task AccessToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentIssuer = securitySettingsFaker.SetNewTokenIssuer().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentIssuer).ValidateAsync();

            // Assert
            securitySettings.TokenIssuer.Should().NotBe(securitySettingsWithDifferentIssuer.TokenIssuer);
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentAudience = securitySettingsFaker.SetNewTokenAudience().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentAudience).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var user = new UserFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentKey = securitySettingsFaker.SetNewAccessTokenKey().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(user);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Signature validation failed.");
        }

        [Theory]
        [InlineData(1)]
        public async Task ValidateAccessToken_Expired_ReturnIsValidFalse(int lifetimeInSeconds)
        {
            // Arrange
            var user = new UserFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().SetAccessTokenLifetime(lifetimeInSeconds).Generate();

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(user);

            Thread.Sleep(lifetimeInSeconds * 1_000);

            TokenValidationResult tokenValidationResult = await accessToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}