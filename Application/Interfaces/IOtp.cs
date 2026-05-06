namespace Application
{
    public interface IOtp
    {
        Task<string> CreateAndSendAsync(string username, string password);
    }
}