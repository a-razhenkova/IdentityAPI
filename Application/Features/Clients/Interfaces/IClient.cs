using Microsoft.AspNetCore.Http;

namespace Application
{
    public interface IClient
    {
        Task<PaginatedReportDto<ClientDto>> SearchAsync(SearchClientQuery query, CancellationToken cancellationToken = default);

        Task<ClientDto> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<string> CreateAsync(CreateClientCommand command, CancellationToken cancellationToken = default);
        Task UpdateAsync(string key, UpdateClientCommand command, CancellationToken cancellationToken = default);
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        Task<string> GetSecretAsync(string key, CancellationToken cancellationToken = default);
        Task<string> UpdateSecretAsync(string key, CancellationToken cancellationToken = default);

        Task CreateSubscription(string clientKey, DateTime expirationDate, IFormFile file, CancellationToken cancellationToken = default);
        Task<FileDto> GetSubscriptionContractAsync(string clientKey, long contractId, CancellationToken cancellationToken = default);
    }
}