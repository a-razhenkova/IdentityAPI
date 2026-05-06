namespace WebApi.V2
{
    public class TokenModel
    {
        // TODO
        /// <summary>
        /// ID token.
        /// </summary>
        /// <example></example>
        //public required string IdToken { get; set; }

        /// <summary>
        /// Access token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJ1c2VybmFtZSI6Iml2YW4uaXZhbm92IiwidXNlclJvbGUiOiJBZG1pbmlzdHJhdG9yIiwidXNlclN0YXR1cyI6IkFjdGl2ZSIsIm5iZiI6MTc0OTUwMTEzNCwiZXhwIjoxNzQ5NTczMTM0LCJpYXQiOjE3NDk1MDExMzQsImlzcyI6IkQxQ0NFNUE5RkQ5ODBCOTlCMkZDM0FGQjg4MThDQTZBRUNBNEU5RDFCRUI0N0FGMUM1OTc4REMyMEVCNTJCMEMiLCJhdWQiOiI4MTA0NTJhODA4YTIyMGM1MjQxNDUyYWJjMDQzNzZlNjZhMWJiNDE1NGU5NTRlYjQ3MjRjNGI4ZmY5Mzk5YmI2In0.f9jOUW8SjO9lFqEpFNzAAZnBgS4k6pqv8QUNx8y9aJg</example>
        public required string AccessToken { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJuYmYiOjE3NTU4Njk0MzAsImV4cCI6MTc1NTg3MDYzMCwiaWF0IjoxNzU1ODY5NDMwLCJpc3MiOiJEMUNDRTVBOUZEOTgwQjk5QjJGQzNBRkI4ODE4Q0E2QUVDQTRFOUQxQkVCNDdBRjFDNTk3OERDMjBFQjUyQjBDIiwiYXVkIjoiODEwNDUyYTgwOGEyMjBjNTI0MTQ1MmFiYzA0Mzc2ZTY2YTFiYjQxNTRlOTU0ZWI0NzI0YzRiOGZmOTM5OWJiNiJ9.jXHGFT0k0MYsqZNPGQcb9SVljMCuu1qE7BdfZuTwqO0</example>
        public required string RefreshToken { get; set; }
    }
}