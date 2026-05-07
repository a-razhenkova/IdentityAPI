using Application;

namespace Tests.Mocks
{
    public class SettingsMock
    {
        public static SecuritySettings CreateBasicSecuritySettings(
            string? issuer = default,
            string? audience = default,
            string accessKey = "Si8lk4k2%Y-UT0~S(pU7YEC56h{K6GXD",
            string refreshKey = "VQJAz9-2cJu4?4|baop4#&E4sBtO0F/f",
            int lifetime = 1200)
        {
            return new SecuritySettings()
            {
                TokenIssuer = string.IsNullOrWhiteSpace(issuer) ? Guid.NewGuid().ToString() : issuer,
                TokenAudience = string.IsNullOrWhiteSpace(audience) ? Guid.NewGuid().ToString() : audience,
                AccessToken = new SecurityTokenSettings()
                {
                    Key = accessKey,
                    LifetimeInSeconds = lifetime
                },
                RefreshToken = new SecurityTokenSettings()
                {
                    Key = refreshKey,
                    LifetimeInSeconds = lifetime
                }
            };
        }
    }
}