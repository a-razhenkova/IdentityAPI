using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Shared;
using System.Data;

namespace Application
{
    public class ClientService : IClient
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginatedReport _paginatedReport;

        public ClientService(
            IOptionsSnapshot<AppSettings> appSettings,
            IUnitOfWork unitOfWork,
            IPaginatedReport paginatedReport)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _paginatedReport = paginatedReport;
        }

        public async Task<PaginatedReportDto<ClientDto>> SearchAsync(SearchClientQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<Client> clientQuery = _unitOfWork.Clients.AsNoTracking().ToQuery();

            if (!string.IsNullOrWhiteSpace(query.Key))
                clientQuery = clientQuery.Where(c => c.Key == query.Key);

            if (!string.IsNullOrWhiteSpace(query.Name))
                clientQuery = clientQuery.Where(c => EF.Functions.Like(c.Name, $"%{query.Name}%"));

            if (query.Status != null)
                clientQuery = clientQuery.Where(c => c.Status.Value == query.Status);

            if (query.CanNotify != null)
                clientQuery = clientQuery.Where(c => c.Right.CanNotify == query.CanNotify);

            clientQuery = clientQuery
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions
                               .Where(s => s.Subscription.ExpirationDate.Date >= DateTime.UtcNow.Date)
                               .OrderByDescending(s => s.Id))
                    .ThenInclude(cs => cs.Subscription)
                    .ThenInclude(s => s.Contract)
                .OrderByDescending(c => c.Id);

            return await _paginatedReport.Prepare(clientQuery, query, ClientMapper.MapToDto, cancellationToken);
        }

        public async Task<ClientDto> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            return client.MapToDto();
        }

        public async Task<string> CreateAsync(CreateClientCommand command, CancellationToken cancellationToken = default)
        {
            Client client = new Client().Map(command);

            await _unitOfWork.Clients.AddAsync(client, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client.Key;
        }

        public async Task UpdateAsync(string key, UpdateClientCommand command, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            Client clientSnapshot = client.DeepCopy();
            client.Map(command);

            bool hasChanges = !client.IsEqual(clientSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges, cancellationToken);
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            if (client.Subscriptions.Count > 0)
            {
                client.Disable();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _unitOfWork.Clients.Remove(client);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<string> GetSecretAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            return client.Secret;
        }

        public async Task<string> UpdateSecretAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == key)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            client.Secret = ClientSecret.Create();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client.Secret;
        }

        public async Task CreateSubscription(string clientKey, DateTime expirationDate, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (expirationDate <= DateTime.UtcNow.Date)
                throw new BadRequestException(AppMessages.InvalidExpirationDate);

            string fileExtension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(fileExtension) || !fileExtension.Equals(FileExtensions.Pdf))
                throw new BadRequestException(AppMessages.InvalidFileType);

            Client client = await _unitOfWork.Clients
                .Where(c => c.Key == clientKey)
                .Include(c => c.Status)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(AppMessages.ClientNotFound);

            DateTime signTimestamp = DateTime.UtcNow;
            string fileName = $"{client.Key}_{signTimestamp:yyyyMMddHHmmssfff}{fileExtension}";
            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContract.Directory, DateTime.UtcNow.Year.ToString(), fileName);

            try
            {
                AesEncryptedFile encryptedFile = await file.CreateAndAesEncryptAsync(contractPath, _appSettings.ClientSubscriptionContract.Key, cancellationToken);

                client.Subscriptions.Add(new ClientSubscription()
                {
                    Subscription = new Subscription()
                    {
                        CreateTimestamp = signTimestamp.Date,
                        ExpirationDate = expirationDate.Date,
                        Contract = new Document()
                        {
                            SignTimestamp = signTimestamp,
                            Name = fileName,
                            Checksum = encryptedFile.Checksum,
                            Type = DocumentTypes.SubscriptionContract,
                            Secret = encryptedFile.Secret
                        }
                    }
                });

                if (!client.IsActivated() && client.IsBlockedWithReason(ClientStatusReasons.ExpiredSubscription))
                    client.Activate();

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                if (File.Exists(contractPath))
                    File.Delete(contractPath);

                throw;
            }
        }

        public async Task<FileDto> GetSubscriptionContractAsync(string clientKey, long contractId, CancellationToken cancellationToken = default)
        {
            Document contract = await _unitOfWork.Documents
                .GetClientSubscriptionContractAsync(clientKey, contractId, cancellationToken)
                ?? throw new NotFoundException(AppMessages.ClientContractNotFound);

            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContract.Directory, contract.SignTimestamp.Year.ToString(), contract.Name);
            byte[] content = await FileExtensions.ReadAndAesDecryptAllBytesAsync(contractPath, _appSettings.ClientSubscriptionContract.Key, contract.Secret, cancellationToken);

            return new FileDto()
            {
                Name = contract.Name,
                Content = content
            };
        }
    }
}