using Microsoft.AspNetCore.Http;
using Shared;

namespace Application
{
    public interface IClient
    {
        Task<PaginatedReport<ClientDto>> SearchAsync(ClientSearchParams clientSearchParams, CancellationToken cancellationToken);

        Task<ClientDto> LoadAsync(string key);
        Task<string> RegisterAsync(ClientDto clientDto);
        Task UpdateAsync(string key, ClientDto clientDto);
        Task DeleteAsync(string key);

        Task<string> LoadSecretAsync(string key);
        Task<string> RefreshSecretAsync(string key);

        Task AddNewSubscription(string clientKey, DateTime expirationDate, IFormFile file);

        Task<FileDto> DownloadContractAsync(string clientKey, long contractId, DocumentTypes documentType);
    }
}