namespace WebApi.V1
{
    public class TokenValidationResultResponse
    {
        /// <summary>
        /// Flag indicating whether the token is valid.
        /// </summary>
        public required bool IsValid { get; set; } = false;

        /// <summary>
        /// Exception if token is invalid.
        /// </summary>
        public string? Exception { get; set; }
    }
}