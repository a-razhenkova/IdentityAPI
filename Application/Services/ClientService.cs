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

        public async Task<PaginatedReport<ClientDto>> SearchAsync(ClientSearchParams clientSearchParams, CancellationToken cancellationToken)
        {
            IQueryable<Client> searchQuery = _unitOfWork.Clients.GetRepo();

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

        public async Task<ClientDto> LoadAsync(string key)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key, loadStatus: true, loadRight: true)
                ?? throw new NotFoundException("Client not found.");

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<string> RegisterAsync(ClientDto clientDto)
        {
            Client client = _mapper.Map<Client>(clientDto);

            await _unitOfWork.Clients.BasicAddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            return client.Key;
        }

        public async Task UpdateAsync(string key, ClientDto clientDto)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key, autoTrack: true, loadStatus: true, loadRight: true)
                ?? throw new NotFoundException("Client not found");

            Client clientSnapshot = client.DeepCopy();
            client = _mapper.Map(clientDto, client);

            bool hasChanges = !client.IsEqual(clientSnapshot);
            await _unitOfWork.SaveChangesAsync(hasChanges);
        }

        public async Task DeleteAsync(string key)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key, autoTrack: true, loadStatus: true, loadSubscriptions: true)
                ?? throw new NotFoundException("Client not found.");

            if (client.Subscriptions.Any())
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = ClientStatusReasons.None;
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                _unitOfWork.Clients.BasicRemove(client);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<string> LoadSecretAsync(string key)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key)
                ?? throw new NotFoundException("Client not found.");

            return client.Secret;
        }

        public async Task<string> RefreshSecretAsync(string key)
        {
            Client client = await _unitOfWork.Clients.GetByKeyAsync(key, autoTrack: true)
                ?? throw new NotFoundException("Client not found.");

            client.Secret = ClientSecret.Create();

            await _unitOfWork.SaveChangesAsync();

            return client.Secret;
        }

        public async Task AddNewSubscription(string clientKey, DateTime expirationDate, IFormFile file)
        {
            if (expirationDate <= DateTime.UtcNow.Date)
                throw new BadRequestException("Invalid expiration date.");

            string fileExtension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(fileExtension) || !fileExtension.Equals(FileExtensions.Pdf))
                throw new BadRequestException("Invalid file type.");

            Client client = await _unitOfWork.Clients.GetByKeyAsync(clientKey, autoTrack: true, loadStatus: true, loadSubscriptions: true)
                ?? throw new NotFoundException("Client not found.");

            DateTime signTimestamp = DateTime.UtcNow;
            string fileName = $"{client.Key}_{signTimestamp:yyyyMMddHHmmssfff}{fileExtension}";
            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContractDirectory, signTimestamp.Year.ToString(), fileName);

            FileExtensions.EnsureFileDirectoryExists(contractPath);

            try
            {
                using FileStream content = File.Create(contractPath);
                await file.CopyToAsync(content);

                client.Subscriptions.Add(new ClientSubscription()
                {
                    Subscription = new Subscription()
                    {
                        CreateTimestamp = expirationDate.Date,
                        ExpirationDate = expirationDate.Date,
                        Contract = new Document()
                        {
                            SignTimestamp = signTimestamp,
                            Name = fileName,
                            Checksum = content.ComputeMd5Checksum(),
                            Type = DocumentTypes.SubscriptionContract
                        }
                    }
                });

                if (client.Status.Reason == ClientStatusReasons.ExpiredSubscription)
                {
                    client.Status.Value = ClientStatuses.Active;
                    client.Status.Reason = ClientStatusReasons.None;
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (File.Exists(contractPath))
                    File.Delete(contractPath);

                throw;
            }
        }

        public async Task<FileDto> DownloadContractAsync(string clientKey, long contractId, DocumentTypes documentType)
        {
            Document contract = await _unitOfWork.Clients.GetSubscriptionContract(clientKey, contractId, documentType)
                ?? throw new NotFoundException("Contract not found.");

            string contractPath = Path.Combine(_appSettings.ClientSubscriptionContractDirectory, contract.SignTimestamp.Year.ToString(), contract.Name);

            return new FileDto()
            {
                Name = contract.Name,
                Content = await File.ReadAllBytesAsync(contractPath)
            };
        }
    }
}