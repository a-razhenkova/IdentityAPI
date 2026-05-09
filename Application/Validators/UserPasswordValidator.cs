using System.Text.RegularExpressions;

namespace Application
{
    public static class UserPasswordValidator
    {
        public static void Validate(string? password, SecuritySettings settings)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password is required.");

            if (!new Regex(settings.PasswordValidationRegex).IsMatch(password))
                throw new BadRequestException($"Password must match the regular expression '{settings.PasswordValidationRegex}'.");
        }
    }
}