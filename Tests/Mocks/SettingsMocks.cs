using Application;

namespace Tests.Mocks
{
    public class SettingsMocks
    {
        public static SecuritySettings CreateBasicSecuritySettings(
            string? issuer = default,
            string? audience = default,
            string key = "Si8lk4k2%Y-UT0~S(pU7YEC56h{K6GXD",
            int lifetime = 1200)
        {
            return new SecuritySettings()
            {
                TokenIssuer = string.IsNullOrWhiteSpace(issuer) ? Guid.NewGuid().ToString() : issuer,
                TokenAudience = string.IsNullOrWhiteSpace(audience) ? Guid.NewGuid().ToString() : audience,
                AccessToken = new SecurityTokenSettings()
                {
                    Key = key,
                    LifetimeInSeconds = lifetime
                }
            };
        }
    }
}