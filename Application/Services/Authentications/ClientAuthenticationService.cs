using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;

namespace Application
{
    public class ClientAuthenticationService : IClientAuthenticator
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;

        public ClientAuthenticationService(IOptionsSnapshot<AppSettings> appSettings,
                                          IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<Client> AuthenticateAsync(string key)
        {
            Client client = await _unitOfWork.Clients.GetQueryAsync(c => c.Key == key, autoTrack: true, loadStatus: true, loadRight: true)
                .Include(c => c.Subscriptions.Where(s => s.Subscription.ExpirationDate >= DateTime.UtcNow.Date))
                .SingleOrDefaultAsync() ?? throw new UnauthorizedException("Invalid credentials.");

            CheckClientStatus(client.Status);

            if (!client.IsInternal && !client.Subscriptions.Any())
            {
                client.Block(ClientStatusReasons.ExpiredSubscription);
                await _unitOfWork.SaveChangesAsync();

                throw new ForbiddenException("Client subscription has expired.");
            }

            return client;
        }

        public async Task<Client> AuthenticateAsync(string key, string secret)
        {
            Client client = await AuthenticateAsync(key);

            bool isSecretValid = client.IsSecretValid(secret);

            await ProcessLoginAttempt(client, isSecretValid);

            if (!isSecretValid)
                throw new UnauthorizedException("Invalid credentials.");

            return client;
        }

        private void CheckClientStatus(ClientStatus status)
        {
            if (status.Value == ClientStatuses.Blocked
             || status.Value == ClientStatuses.Disabled)
            {
                throw new ForbiddenException($"Client status is '{status.Value}'.");
            }
        }

        private async Task ProcessLoginAttempt(Client client, bool isSecretValid)
        {
            if (isSecretValid)
            {
                client.WrongLoginAttemptsCounter = 0;
            }
            else
            {
                client.WrongLoginAttemptsCounter++;

                if (client.WrongLoginAttemptsCounter >= _appSettings.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                    client.Block(ClientStatusReasons.MaxWrongLoginAttemptsReached);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}