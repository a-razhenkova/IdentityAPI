using Application;
using Domain;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using Tests.Mocks;

namespace UnitTests
{
    public class ClientTokenTests
    {
        [Theory]
        [InlineData("Ivan Ivanov", ClientStatuses.Active, true, false)]
        [InlineData("Petar Petrov", ClientStatuses.Blocked, false, true)]
        public void AccessToken_Create_ReturnTokenWithClaims(string name, ClientStatuses clientStatus, bool isInternal, bool canNotify)
        {
            // Arrange
            string issuer = Guid.NewGuid().ToString();
            string audience = Guid.NewGuid().ToString();
            int tokenLifetime = 1200;

            var securitySettings = SettingsMocks.CreateBasicSecuritySettings(issuer: issuer, audience: audience, lifetime: tokenLifetime);
            var client = ClientMocks.CreateBasicClient(name: name, clientStatus: clientStatus, isInternal: isInternal, canNotify: canNotify);

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);

            // Assert
            var cfg = new AccessToken(accessToken, securitySettings);

            JwtSecurityToken? jwt = cfg.Decode();
            jwt.Should().NotBeNull();

            jwt.Claims.Count().Should().Be(9);

            cfg.GetClaim(jwt, TokenClaim.Issuer).Should().Be(issuer);
            cfg.GetClaim(jwt, TokenClaim.Audience).Should().Be(audience);
            cfg.GetClaim(jwt, TokenClaim.ClientId).Should().Be(client.Key);
            cfg.GetClaim(jwt, TokenClaim.ClientStatus).Should().Be(clientStatus.ToString().ToUpper());
            cfg.GetClaim(jwt, TokenClaim.IsInternalClient).Should().Be(isInternal.ToString().ToLower());
            cfg.GetClaim(jwt, TokenClaim.CanNotify).Should().Be(canNotify.ToString().ToLower());
        }

        [Fact]
        public async Task AccessToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var securitySettings = SettingsMocks.CreateBasicSecuritySettings();
            var client = ClientMocks.CreateBasicClient();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task AccessToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMocks.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMocks.CreateBasicSecuritySettings(issuer: Guid.NewGuid().ToString(),
                securitySettings.TokenAudience, securitySettings.AccessToken.Key, securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMocks.CreateBasicClient();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMocks.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMocks.CreateBasicSecuritySettings(securitySettings.TokenIssuer,
                audience: Guid.NewGuid().ToString(), securitySettings.AccessToken.Key, securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMocks.CreateBasicClient();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMocks.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMocks.CreateBasicSecuritySettings(securitySettings.TokenIssuer,
                securitySettings.TokenAudience, key: "VQJAz9-2cJu4?4|baop4#&E4sBtO0F/f", securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMocks.CreateBasicClient();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentKey).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Signature validation failed.");
        }

        [Fact]
        public async Task ValidateAccessToken_Expired_ReturnIsValidFalse()
        {
            // Arrange
            var securitySettings = SettingsMocks.CreateBasicSecuritySettings(lifetime: 1);
            var client = ClientMocks.CreateBasicClient();

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);

            Thread.Sleep(1_000);

            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettings).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}