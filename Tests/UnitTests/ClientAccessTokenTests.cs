using Application;
using Domain;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using Tests.Mocks;

namespace UnitTests
{
    public class ClientAccessTokenTests
    {
        [Theory]
        [InlineData("read API", ClientStatuses.Active, true, false)]
        [InlineData("write API", ClientStatuses.Blocked, false, true)]
        public void AccessToken_Create_ReturnTokenWithClaims(string name, ClientStatuses clientStatus, bool isInternal, bool canNotify)
        {
            // Arrange
            string issuer = Guid.NewGuid().ToString();
            string audience = Guid.NewGuid().ToString();

            var securitySettings = SettingsMock.CreateBasicSecuritySettings(issuer: issuer, audience: audience);
            var client = ClientMock.CreateBasicClient(name: name, status: clientStatus, isInternal: isInternal, canNotify: canNotify);

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
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var client = ClientMock.CreateBasicClient();

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
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: Guid.NewGuid().ToString(),
                audience: securitySettings.TokenAudience, accessKey: securitySettings.AccessToken.Key, lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMock.CreateBasicClient();

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
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: securitySettings.TokenIssuer,
                audience: Guid.NewGuid().ToString(), accessKey: securitySettings.AccessToken.Key, lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMock.CreateBasicClient();

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
            var securitySettings = SettingsMock.CreateBasicSecuritySettings();
            var securitySettingsWithDifferentKey = SettingsMock.CreateBasicSecuritySettings(issuer: securitySettings.TokenIssuer,
                audience: securitySettings.TokenAudience, accessKey: "VQJAz9-2cJu4?4|baop4#&E4sBtO0F/f", lifetime: securitySettings.AccessToken.LifetimeInSeconds);
            var client = ClientMock.CreateBasicClient();

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
            var securitySettings = SettingsMock.CreateBasicSecuritySettings(lifetime: 1);
            var client = ClientMock.CreateBasicClient();

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