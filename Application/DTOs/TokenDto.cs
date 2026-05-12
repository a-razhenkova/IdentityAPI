namespace Application
{
    public class TokenDto
    {
        public required string AccessToken { get; set; }

        public string? RefreshToken { get; set; }
    }
}