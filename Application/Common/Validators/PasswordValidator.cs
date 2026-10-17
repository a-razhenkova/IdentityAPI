using System.Text.RegularExpressions;

namespace Application
{
    public static class PasswordValidator
    {
        public static string? Validate(string? password, AppSettings appSettings)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password is required.";

            if (!new Regex(appSettings.Security.PasswordValidationRegex).IsMatch(password))
                return $"Password must match the regular expression '{appSettings.Security.PasswordValidationRegex}'.";

            return default;
        }
    }
}