namespace WebApi.V2
{
    public class TokenModel
    {
        /// <summary>
        /// Access token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJ1c2VybmFtZSI6Iml2YW4uaXZhbm92IiwidXNlclJvbGUiOiJBZG1pbmlzdHJhdG9yIiwidXNlclN0YXR1cyI6IkFjdGl2ZSIsIm5iZiI6MTc3ODA5MDk1MywiZXhwIjoxNzc4MDkyMTUzLCJpYXQiOjE3NzgwOTA5NTMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAxIiwiYXVkIjoiQWxla3NhbmRyaW5hIFJhemhlbmtvdmEifQ.EKDIqIkGZkj5MKyr-6wsykPxs-s-Pl8zSAw7rDFrvG0</example>
        public required string AccessToken { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJuYmYiOjE3NzgwOTA5NTMsImV4cCI6MTc3ODI2Mzc1MywiaWF0IjoxNzc4MDkwOTUzLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMwMSIsImF1ZCI6IkFsZWtzYW5kcmluYSBSYXpoZW5rb3ZhIn0.IUQm6Ptb2QOuNeRVG55WmisNKdojFIRDEB0clEIy-58</example>
        public required string RefreshToken { get; set; }
    }
}