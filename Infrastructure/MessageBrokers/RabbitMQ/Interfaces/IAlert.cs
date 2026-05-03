using Infrastructure.RabbitMq;

namespace Infrastructure
{
    public interface IAlert
    {
        Task AddUserPasswordChangedAlertAsync (UserPasswordChangedAlertDto alertDto);
    }
}