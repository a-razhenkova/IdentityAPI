using System.ComponentModel.DataAnnotations;

namespace Application
{
    public record SecuritySettings
    {
        public RateLimiterSettings RateLimiter { get; init; } = new();

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int DefaultMaxWrongLoginAttemptsBeforeBlock { get; init; } = 3;

        [Required]
        public string PasswordValidationRegex { get; init; } = string.Empty;

        [Required]
        public string TokenIssuer { get; init; } = string.Empty;

        [Required]
        public string TokenAudience { get; init; } = string.Empty;

        public SecurityTokenSettings AccessToken { get; init; } = new();

        public SecurityTokenSettings RefreshToken { get; init; } = new();

        public SecurityTokenSettings EmailVerificationToken { get; init; } = new();

        public MultiFactorAuthSettings MultiFactorAuth { get; init; } = new();
    }

    public record RateLimiterSettings
    {
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int WindowInSeconds { get; init; } = 30;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int RequestsPerWindow { get; init; } = 15;
    }

    public record SecurityTokenSettings
    {
        [MinLength(16)]
        public string Key { get; init; } = string.Empty;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int LifetimeInSeconds { get; init; }
    }

    public record MultiFactorAuthSettings
    {
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int LifetimeInSeconds { get; init; } = 30;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int DefaultMaxWrongLoginAttemptsBeforeBlock { get; init; } = 3;
    }
}