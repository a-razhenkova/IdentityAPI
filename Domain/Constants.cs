using Shared;

namespace Domain
{
    public static class ClientConstants
    {
        public const int KeyMaxLength = Constants.UidLength;
        public const int SecretMaxLength = Constants.UidLength;
        public const int NameMaxLength = Constants.UidLength;
        public const int StatusNoteMaxLength = 256;

        public const string NameRegex = @"^[a-zA-Z][\w .,-]+[a-zA-Z]$";
        public const string StatusNoteRegex = Constants.FreeTextRegex;
    }

    public static class UserConstants
    {
        public const int PublicIdMaxLength = Constants.UidLength;
        public const int UsernameMinLength = 6;
        public const int UsernameMaxLength = 64;
        public const int OtpKeyMaxLength = 64;
        public const int EmailMaxLength = Constants.EmailMaxLength;
        public const int RawPasswordMaxLength = 128;
        public const int PasswordHashMaxLength = 256;
        public const int PasswordSecretMaxLength = 128;
        public const int StatusNoteMaxLength = 256;

        public const string UsernameRegex = @"^[a-zA-Z][\w.-]+[a-zA-Z]$";
        public const string StatusNoteRegex = Constants.FreeTextRegex;
    }

    public static class DocumentConstants
    {
        public const int NameMaxLength = 128;
        public const int ChecksumMaxLength = 64;
        public const int KeyMaxLength = 64;
        public const int SecretMaxLength = 64;
    }
}