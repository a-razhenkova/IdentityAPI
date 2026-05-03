namespace Business
{
    public interface IOtp
    {
        Task<string> CreateAndSendAsync(string username, string password);
    }
}