using Business.RabbitMq;

namespace Business
{
    public interface IAlert
    {
        Task AddUserPasswordChangedAlertAsync (UserPasswordChangedAlertDto alertDto);
    }
}