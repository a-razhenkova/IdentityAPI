namespace Application
{
    public class UserEventBase
    {
        public required long UserId { get; set; }

        public required string UserEmail { get; set; }

        public required string Username { get; set; }
    }
}