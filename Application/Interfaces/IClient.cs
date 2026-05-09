using Domain;
using Microsoft.AspNetCore.Http;

namespace Application
{
    public interface IClient
    {
        Task<PaginatedReport<ClientDto>> SearchAsync(ClientSearchParams clientSearchParams, CancellationToken cancellationToken = default);

        Task<ClientDto> LoadAsync(string key, CancellationToken cancellationToken = default);
        Task<string> RegisterAsync(ClientDto clientDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(string key, ClientDto clientDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        Task<string> LoadSecretAsync(string key, CancellationToken cancellationToken = default);
        Task<string> RefreshSecretAsync(string key, CancellationToken cancellationToken = default);

        Task AddNewSubscription(string clientKey, DateTime expirationDate, IFormFile file, CancellationToken cancellationToken = default);
        Task<FileDto> DownloadSubscriptionContractAsync(string clientKey, long contractId, CancellationToken cancellationToken = default);
    }
}