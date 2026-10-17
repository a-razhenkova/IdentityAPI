namespace Application
{
    public interface IEmail
    {
        Task VerifyToken(string token, CancellationToken cancellationToken = default);
    }
}