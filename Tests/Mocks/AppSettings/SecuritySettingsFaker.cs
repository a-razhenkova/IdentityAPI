using Application;
using Bogus;

namespace Tests.Mocks
{
    public class SecuritySettingsFaker : Faker<SecuritySettings>
    {
        private string? _tokenIssuer;
        private string? _tokenAudience;

        private string? _accessTokenKey;
        private int _accessTokenLifetime;

        private string? _refreshTokenKey;
        private int _refreshTokenLifetime;

        public SecuritySettingsFaker()
        {
            var faker = new Faker();
            _tokenIssuer = faker.Random.Uuid().ToString();
            _tokenAudience = faker.Random.Uuid().ToString();

            _accessTokenKey = faker.Random.Uuid().ToString();
            _accessTokenLifetime = faker.Random.Int(1_000, 100_000);

            _refreshTokenKey = faker.Random.Uuid().ToString();
            _refreshTokenLifetime = faker.Random.Int(1_000, 100_000);

            RuleFor(s => s.TokenIssuer, f => _tokenIssuer);
            RuleFor(s => s.TokenAudience, f => _tokenAudience);
            RuleFor(s => s.AccessToken, f => new SecurityTokenSettings()
            {
                Key = _accessTokenKey,
                LifetimeInSeconds = _accessTokenLifetime
            });
            RuleFor(s => s.RefreshToken, f => new SecurityTokenSettings()
            {
                Key = _refreshTokenKey,
                LifetimeInSeconds = _refreshTokenLifetime
            });
        }

        public SecuritySettingsFaker SetNewTokenIssuer()
        {
            _tokenIssuer = new Faker().Random.Uuid().ToString();
            return this;
        }

        public SecuritySettingsFaker SetNewTokenAudience()
        {
            _tokenAudience = new Faker().Random.Uuid().ToString();
            return this;
        }

        public SecuritySettingsFaker SetNewAccessTokenKey()
        {
            _accessTokenKey = new Faker().Random.Uuid().ToString();
            return this;
        }

        public SecuritySettingsFaker SetNewRefreshTokenKey()
        {
            _refreshTokenKey = new Faker().Random.Uuid().ToString();
            return this;
        }

        public SecuritySettingsFaker SetAccessTokenLifetime(int lifetimeInSeconds)
        {
            _accessTokenLifetime = lifetimeInSeconds;
            return this;
        }

        public SecuritySettingsFaker SetRefreshTokenLifetime(int lifetimeInSeconds)
        {
            _refreshTokenLifetime = lifetimeInSeconds;
            return this;
        }
    }
}