using Application;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace WebApi.V1
{
    [AuthorizeUser(UserRoles.Administrator)]
    [Route("api/v1/[controller]")]
    public class ClientsController : JsonApiControllerBase
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public ClientsController(IClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of clients.
        /// </summary>
        /// <param name="searchParams">Search parameters for filtering clients.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated report of clients matching the search criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedReport<ClientModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchClientAsync([FromQuery] ClientSearchParams searchParams, CancellationToken cancellationToken)
        {
            PaginatedReport<ClientDto> searchResult = await _client.SearchAsync(searchParams, cancellationToken);
            return Ok(_mapper.Map<PaginatedReport<ClientModel>>(searchResult));
        }

        /// <summary>
        /// Retrieves a client.
        /// </summary>
        /// <param name="key">The key of the client to be retrieved.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The client details if found.</returns>
        [HttpGet("{key}")]
        [ProducesResponseType(typeof(ClientModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoadClientAsync(string key, CancellationToken cancellationToken)
        {
            ClientDto clientDto = await _client.LoadAsync(key, cancellationToken);
            return Ok(_mapper.Map<ClientModel>(clientDto));
        }

        /// <summary>
        /// Registers a new client.
        /// </summary>
        /// <param name="requestModel">The model containing client registration details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The key of the registered client.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SimpleResponseModel<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterClientAsync(ClientRegistrationModel requestModel, CancellationToken cancellationToken)
        {
            string key = await _client.RegisterAsync(_mapper.Map<ClientDto>(requestModel), cancellationToken);
            return Created(string.Empty, new SimpleResponseModel<string>(key));
        }

        /// <summary>
        /// Updates an existing client's details.
        /// </summary>
        /// <param name="key">The key of the client to be updated.</param>
        /// <param name="requestModel">The model containing the updated client details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClientAsync(string key, ClientUpdateModel requestModel, CancellationToken cancellationToken)
        {
            await _client.UpdateAsync(key, _mapper.Map<ClientDto>(requestModel), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Deletes a client.
        /// </summary>
        /// <param name="key">The key of the client to be deleted.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteClientAsync(string key, CancellationToken cancellationToken)
        {
            await _client.DeleteAsync(key, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Retrieves the secret of a client.
        /// </summary>
        /// <param name="key">The key of the client whose secret is to be retrieved.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The secret of the client.</returns>
        [HttpGet("{key}/secret"), SensitiveData(IsRequestSensitive = false, IsResponseSensitive = true)]
        [ProducesResponseType(typeof(SimpleResponseModel<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoadClientSecretAsync(string key, CancellationToken cancellationToken)
        {
            string clientSecret = await _client.LoadSecretAsync(key, cancellationToken);
            return Ok(new SimpleResponseModel<string>(clientSecret));
        }

        /// <summary>
        /// Refreshes the secret for a client.
        /// </summary>
        /// <param name="key">The key of the client whose secret is to be refreshed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPatch("{key}/secret"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshClientSecretAsync(string key, CancellationToken cancellationToken)
        {
            await _client.RefreshSecretAsync(key, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Adds new subscription for a client.
        /// </summary>
        /// <param name="key">The key of the client whose subscription is to be renewed.</param>
        /// <param name="expirationDate">The expiration date for the subscription.</param>
        /// <param name="file">The contract file to be uploaded as part of the renewal.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{key}/subscriptions"), SensitiveData]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddNewClientSubscriptionAsync(string key, [FromForm] DateTime expirationDate, IFormFile file, CancellationToken cancellationToken)
        {
            await _client.AddNewSubscription(key, expirationDate, file, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Downloads the subscription contract for a specific client.
        /// </summary>
        /// <param name="key">The key of the client whose contract is to be retrieved.</param>
        /// <param name="id">The ID of the contract to download.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The contract.</returns>
        [HttpGet("{key}/subscriptions/contracts/{id}"), SensitiveData(IsRequestSensitive = false, IsResponseSensitive = true)]
        [Produces(MediaTypeNames.Application.Pdf)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadClientSubscriptionContractAsync(string key, long id, CancellationToken cancellationToken)
        {
            FileDto file = await _client.DownloadSubscriptionContractAsync(key, id, cancellationToken);
            return File(file.Content, MediaTypeNames.Application.Pdf, file.Name);
        }
    }
}