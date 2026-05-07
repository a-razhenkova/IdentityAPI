namespace WebApi.V1
{
    public class TokenModel
    {
        /// <summary>
        /// Access token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRJZCI6ImRiYTFkMjVhLTAwNjItNDllNy1iNGYwLTMxMjI0YTY5ZjllNCIsImNsaWVudFN0YXR1cyI6IkFDVElWRSIsImlzSW50ZXJuYWxDbGllbnQiOnRydWUsImNhbk5vdGlmeSI6dHJ1ZSwibmJmIjoxNzc4MTYwNjg4LCJleHAiOjE3NzgxNjE4ODgsImlhdCI6MTc3ODE2MDY4OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDEiLCJhdWQiOiJBbGVrc2FuZHJpbmEgUmF6aGVua292YSJ9._N_-fZJke50_z137XRpZRXw3qhbKjEvbu22aapMIPB0</example>
        public required string AccessToken { get; set; }
    }
}