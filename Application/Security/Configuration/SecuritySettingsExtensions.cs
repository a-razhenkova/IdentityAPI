using System.Text.RegularExpressions;

namespace Application
{
    public static class SecuritySettingsExtensions
    {
        extension(SecuritySettings settings)
        {
            public string? ValidatePassword(string? password)
            {
                if (string.IsNullOrWhiteSpace(password))
                    return "Password is required.";

                if (!new Regex(settings.PasswordValidationRegex).IsMatch(password))
                    return $"Password must match the regular expression '{settings.PasswordValidationRegex}'.";

                return default;
            }
        }
    }
}