using Application;
using Domain;
using FluentAssertions;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using Xunit.Priority;

namespace UnitTests
{
    public class ClientTokenTests
    {
        [Priority(1)]
        [Theory]
        [InlineData("Ivan Ivanov", ClientStatuses.Active, true, false)]
        [InlineData("Petar Petrov", ClientStatuses.Blocked, false, true)]
        public void AccessToken_ClaimsCheck(string name, ClientStatuses clientStatus, bool isInternal, bool canNotify)
        {
            // Arrange
            string issuer = Guid.NewGuid().ToString();
            string audience = Guid.NewGuid().ToString();

            var securitySettings = new SecuritySettings()
            {
                TokenIssuer = issuer,
                TokenAudience = audience,
                AccessToken = new SecurityTokenSettings()
                {
                    Key = Guid.NewGuid().ToString(),
                    LifetimeInSeconds = 1200
                }
            };

            var client = new Client()
            {
                Id = new Random().Next(100, 1000),
                Name = name,
                Key = ClientKey.Create(),
                Secret = ClientSecret.Create(),
                WrongLoginAttemptsCounter = 0,
                IsInternal = isInternal,
                Status = new ClientStatus()
                {
                    Id = new Random().Next(100, 1000),
                    Value = clientStatus,
                    Reason = ClientStatusReasons.None
                },
                Right = new ClientRight()
                {
                    CanNotify = canNotify
                }
            };

            // Act
            string accessToken = new AccessToken(securitySettings).Create(client);

            // Assert
            var cfg = new AccessToken(accessToken, securitySettings);

            JwtSecurityToken? jwt = cfg.Decode();
            jwt.Should().NotBeNull();

            jwt.Claims.Count().Should().Be(9);

            // TODO: validate expiration time
            cfg.GetClaim(jwt, TokenClaim.Issuer).Should().Be(issuer);
            cfg.GetClaim(jwt, TokenClaim.Audience).Should().Be(audience);
            cfg.GetClaim(jwt, TokenClaim.ClientId).Should().Be(client.Key);
            cfg.GetClaim(jwt, TokenClaim.ClientStatus).Should().Be(clientStatus.ToString().ToUpper());
            cfg.GetClaim(jwt, TokenClaim.IsInternalClient).Should().Be(isInternal.ToString().ToLower());
            cfg.GetClaim(jwt, TokenClaim.CanNotify).Should().Be(canNotify.ToString().ToLower());
        }
    }
}