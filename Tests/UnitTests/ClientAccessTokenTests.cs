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
        [InlineData(ClientStatuses.Active, true, false)]
        [InlineData(ClientStatuses.Blocked, false, true)]
        public void AccessToken_Create_ReturnTokenWithClaims(ClientStatuses clientStatus, bool isInternalClient, bool canClientNotify)
        {
            // Arrange
            var securitySettings = new SecuritySettingsFaker().Generate();

            var client = new ClientFaker().Generate();
            client.Status.Value = clientStatus;
            client.IsInternal = isInternalClient;
            client.Right.CanNotify = canClientNotify;

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(client);

            // Assert
            JwtSecurityToken? jwt = accessToken.Decode();
            jwt.Should().NotBeNull();
            jwt.Claims.Count().Should().Be(9);

            accessToken.GetClaim(jwt, TokenClaim.Issuer).Should().Be(securitySettings.TokenIssuer);
            accessToken.GetClaim(jwt, TokenClaim.Audience).Should().Be(securitySettings.TokenAudience);
            accessToken.GetClaim(jwt, TokenClaim.ClientId).Should().Be(client.Key);
            accessToken.GetClaim(jwt, TokenClaim.ClientStatus).Should().Be(clientStatus.ToString().ToUpper());
            accessToken.GetClaim(jwt, TokenClaim.IsInternalClient).Should().Be(isInternalClient.ToString().ToLower());
            accessToken.GetClaim(jwt, TokenClaim.CanClientNotify).Should().Be(canClientNotify.ToString().ToLower());
        }

        [Fact]
        public async Task AccessToken_Valid_ReturnIsValidTrue()
        {
            // Arrange
            var client = new ClientFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(client);

            TokenValidationResult tokenValidationResult = await accessToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task AccessToken_WithWrongIssuer_ReturnIsValidFalse()
        {
            // Arrange
            var client = new ClientFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentIssuer = securitySettingsFaker.SetNewTokenIssuer().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentIssuer).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Issuer validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongAudience_ReturnIsValidFalse()
        {
            // Arrange
            var client = new ClientFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentAudience = securitySettingsFaker.SetNewTokenAudience().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(client);
            TokenValidationResult tokenValidationResult = await new AccessToken(accessToken, securitySettingsWithDifferentAudience).ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Audience validation failed.");
        }

        [Fact]
        public async Task AccessToken_WithWrongKey_ReturnIsValidFalse()
        {
            // Arrange
            var client = new ClientFaker().Generate();

            var securitySettingsFaker = new SecuritySettingsFaker();
            var securitySettings = securitySettingsFaker.Generate();
            var securitySettingsWithDifferentKey = securitySettingsFaker.SetNewAccessTokenKey().Generate();

            // Act
            var accessToken = new AccessToken(securitySettings).Create(client);
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
            var client = new ClientFaker().Generate();
            var securitySettings = new SecuritySettingsFaker().SetAccessTokenLifetime(lifetimeInSeconds).Generate();

            // Act
            var accessToken = new AccessToken(securitySettings);
            accessToken.Create(client);

            Thread.Sleep(lifetimeInSeconds * 1_000);

            TokenValidationResult tokenValidationResult = await accessToken.ValidateAsync();

            // Assert
            tokenValidationResult.IsValid.Should().BeFalse();
            tokenValidationResult.Exception.Message.Should().Contain("Lifetime validation failed.");
        }
    }
}