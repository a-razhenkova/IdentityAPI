using Domain;

namespace Application
{
    public static class AppMessages
    {
        public const string InvalidCredentials = "Invalid credentials.";
        public const string InvalidPassword = "Invalid password.";
        public const string InvalidExpirationDate = "Invalid expiration date.";

        public const string InvalidToken = "Invalid token.";
        public const string InvalidTokenFormat = "Invalid token format.";

        public const string InvalidCode = "Invalid code.";
        public const string InvalidVerificationCode = "Invalid verification code.";
        public const string ExpiredCode = "The code has expired.";

        public const string InvalidFilePath = "Invalid file path.";
        public const string InvalidFileType = "Invalid file type.";
        public static string DirectoryPathNotFound(string path) => $"Directory '{path}' not found.";

        public static string ClientStatusIs(ClientStatuses status) => $"The client status is '{status}'.";
        public const string ClientNotFound = "The client was not found.";
        public const string ClientContractNotFound = "The client contract was not found.";
        public const string ExpiredClientSubscription = "The client subscription has expired.";

        public static string UserStatusIs(UserStatuses status) => $"The user status is '{status}'.";
        public const string UserNotFound = "The user was not found.";
        public const string UserEmailNotFound = "The user has not provided an email address.";
        public static string UserAlreadyExists(string username) => $"A user with username '{username}' already exists.";
        public const string UserCannotReceiveOtp = "The user cannot receive OTP.";
    }
}