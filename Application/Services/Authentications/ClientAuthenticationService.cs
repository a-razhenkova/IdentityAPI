using Domain;
using Microsoft.Extensions.Options;

namespace Application
{
    public class ClientAuthenticationService : IClientAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;

        public ClientAuthenticationService(
            IOptionsSnapshot<AppSettings> appSettings,
            IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<Client> AuthenticateAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions
                    .Where(s => s.Subscription.ExpirationDate >= DateTime.UtcNow.Date))
                .SingleOrDefaultAsync(cancellationToken) ?? throw new UnauthorizedException(AppMessages.InvalidCredentials);

            if (!client.IsActivated())
                throw new ForbiddenException(AppMessages.ClientStatusIs(client.Status.Value));

            if (!client.IsInternal && !client.Subscriptions.Any())
            {
                client.Block(ClientStatusReasons.ExpiredSubscription);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ForbiddenException(AppMessages.ExpiredClientSubscription);
            }

            return client;
        }

        public async Task<Client> AuthenticateAsync(string key, string secret, CancellationToken cancellationToken = default)
        {
            Client client = await AuthenticateAsync(key, cancellationToken);

            if (!client.Secret.Equals(secret))
            {
                ProcessWrongLoginAttempt(client);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException(AppMessages.InvalidCredentials);
            }

            client.WrongLoginAttemptsCounter = 0;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client;
        }

        private void ProcessWrongLoginAttempt(Client client)
        {
            client.WrongLoginAttemptsCounter++;

            if (client.WrongLoginAttemptsCounter >= _appSettings.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                client.Block(ClientStatusReasons.MaxWrongLoginAttemptsReached);
        }
    }
}