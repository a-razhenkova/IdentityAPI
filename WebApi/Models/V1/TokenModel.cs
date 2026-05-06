namespace WebApi.V1
{
    public class TokenModel
    {
        /// <summary>
        /// Access token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRJZCI6ImRiYTFkMjVhLTAwNjItNDllNy1iNGYwLTMxMjI0YTY5ZjllNCIsImlzSW50ZXJuYWxDbGllbnQiOnRydWUsImNhbk5vdGlmeSI6dHJ1ZSwibmJmIjoxNzc4MDkwOTEwLCJleHAiOjE3NzgwOTIxMTAsImlhdCI6MTc3ODA5MDkxMCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDEiLCJhdWQiOiJBbGVrc2FuZHJpbmEgUmF6aGVua292YSJ9.IQ-0GHQM_17Rn67b34995mmO5SCfAOPCxjxPMfPBnuo</example>
        public required string AccessToken { get; set; }
    }
}