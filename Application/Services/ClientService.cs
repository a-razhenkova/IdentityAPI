using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using System.Data;

namespace Application
{
    public class ClientService : IClient
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginatedReport _paginatedReport;

        public ClientService(IOptionsSnapshot<AppSettings> appSettings,
                            IUnitOfWork unitOfWork,
                            IMapper mapper,
                            IPaginatedReport paginatedReport)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginatedReport = paginatedReport;
        }

        public async Task<PaginatedReport<ClientDto>> SearchAsync(ClientSearchParams clientSearchParams, CancellationToken cancellationToken = default)
        {
            IQueryable<Client> searchQuery = _unitOfWork.Clients.Init().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(clientSearchParams.Key))
            {
                searchQuery = searchQuery.Where(c => c.Key == clientSearchParams.Key);
            }

            if (!string.IsNullOrWhiteSpace(clientSearchParams.Name))
            {
                searchQuery = searchQuery.Where(c => EF.Functions.Like(c.Name, $"%{clientSearchParams.Name}%"));
            }
            if (clientSearchParams.Status is not null)
            {
                searchQuery = searchQuery.Where(c => c.Status.Value == clientSearchParams.Status);
            }

            if (clientSearchParams.CanNotify is not null)
            {
                searchQuery = searchQuery.Where(c => c.Right.CanNotify == clientSearchParams.CanNotify);
            }

            searchQuery = searchQuery
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions
                               .Where(s => s.Subscription.ExpirationDate.Date >= DateTime.UtcNow.Date)
                               .OrderByDescending(s => s.Id))
                    .ThenInclude(cs => cs.Subscription)
                    .ThenInclude(s => s.Contract)
                .OrderByDescending(c => c.Id);

            return await _paginatedReport.Prepare<Client, ClientDto>(searchQuery, clientSearchParams, cancellationToken);
        }

        public async Task<ClientDto> LoadAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .WhereKeyEquals(key, autoTrack: false)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Client not found.");

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<string> RegisterAsync(ClientDto clientDto, CancellationToken cancellationToken = default)
        {
            Client client = _mapper.Map<Client>(clientDto);

            await _unitOfWork.Clients.BasicAddAsync(client, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client.Key;
        }

        public async Task UpdateAsync(string key, ClientDto clientDto, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .WhereKeyEquals(key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Client not found");

            Client clientSnapshot = client.DeepCopy();
            client = _mapper.Map(clientDto, client);

            bool hasChanges = !client.IsEqual(clientSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges, cancellationToken);
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients
                .WhereKeyEquals(key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Client not found.");

            if (client.Subscriptions.Any())
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = ClientStatusReasons.None;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _unitOfWork.Clients.BasicRemove(client);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<string> LoadSecretAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients.GetByKeyWithNoTrackingAsync(key, cancellationToken)
                ?? throw new NotFoundException("Client not found.");

            return client.Secret;
        }

        public async Task<string> RefreshSecretAsync(string key, CancellationToken cancellationToken = default)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key)
                ?? throw new NotFoundException("Client not found.");

            client.Secret = ClientSecretHandler.Create();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client.Secret;
        }

        public async Task AddNewSubscription(string clientKey, DateTime expirationDate, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (expirationDate <= DateTime.UtcNow.Date)
                throw new BadRequestException("Invalid expiration date.");

            string fileExtension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(fileExtension) || !fileExtension.Equals(FileExtensions.Pdf))
                throw new BadRequestException("Invalid file type.");

            Client client = await _unitOfWork.Clients
                .WhereKeyEquals(clientKey)
                .Include(c => c.Status)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Client not found.");

            DateTime signTimestamp = DateTime.UtcNow;
            string fileName = $"{client.Key}_{signTimestamp:yyyyMMddHHmmssfff}{fileExtension}";
            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContractDirectory, signTimestamp.Year.ToString(), fileName);

            FileExtensions.EnsureFileDirectoryExists(contractPath);

            try
            {
                using FileStream content = File.Create(contractPath);
                await file.CopyToAsync(content, cancellationToken);

                client.CreateNewSubscription(expirationDate, content, fileExtension);

                if (client.Status.Reason == ClientStatusReasons.ExpiredSubscription)
                {
                    client.Status.Value = ClientStatuses.Active;
                    client.Status.Reason = ClientStatusReasons.None;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                if (File.Exists(contractPath))
                    File.Delete(contractPath);

                throw;
            }
        }

        public async Task<FileDto> DownloadSubscriptionContractAsync(string clientKey, long contractId, CancellationToken cancellationToken = default)
        {
            Document contract = await _unitOfWork.Clients.GetSubscriptionContractWithNoTrackingAsync(clientKey, contractId, cancellationToken)
                ?? throw new NotFoundException("Contract not found.");

            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContractDirectory, contract.SignTimestamp.Year.ToString(), contract.Name);

            return new FileDto()
            {
                Name = contract.Name,
                Content = await File.ReadAllBytesAsync(contractPath, cancellationToken)
            };
        }
    }
}