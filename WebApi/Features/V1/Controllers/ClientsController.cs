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
        /// <param name="request">Search parameters for filtering clients.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated report of clients matching the search criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedReportResponse<ClientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchClientAsync([FromQuery] SearchClientRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<SearchClientQuery>(request);
            PaginatedReportDto<ClientDto> searchResult = await _client.SearchAsync(command, cancellationToken);
            return Ok(_mapper.Map<PaginatedReportDto<ClientResponse>>(searchResult));
        }

        /// <summary>
        /// Retrieves a client.
        /// </summary>
        /// <param name="key">The key of the client to be retrieved.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The client details if found.</returns>
        [HttpGet("{key}")]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientAsync(string key, CancellationToken cancellationToken)
        {
            ClientDto clientDto = await _client.GetAsync(key, cancellationToken);
            return Ok(_mapper.Map<ClientResponse>(clientDto));
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="request">Contains client registration details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The key of the registered client.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SimpleResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateClientCommand>(request);
            string key = await _client.CreateAsync(command, cancellationToken);

            var response = new SimpleResponse<string>(key);
            return Created(string.Empty, response);
        }

        /// <summary>
        /// Updates an existing client's details.
        /// </summary>
        /// <param name="key">The key of the client to be updated.</param>
        /// <param name="request">Contains the updated client details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClientAsync(string key, UpdateClientRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateClientCommand>(request);
            await _client.UpdateAsync(key, command, cancellationToken);
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
        [ProducesResponseType(typeof(SimpleResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientSecretAsync(string key, CancellationToken cancellationToken)
        {
            string clientSecret = await _client.GetSecretAsync(key, cancellationToken);
            return Ok(new SimpleResponse<string>(clientSecret));
        }

        /// <summary>
        /// Updates the secret for a client.
        /// </summary>
        /// <param name="key">The key of the client whose secret is to be refreshed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPatch("{key}/secret"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshClientSecretAsync(string key, CancellationToken cancellationToken)
        {
            await _client.UpdateSecretAsync(key, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Creates a new subscription for a client.
        /// </summary>
        /// <param name="key">The key of the client whose subscription is to be renewed.</param>
        /// <param name="expirationDate">The expiration date for the subscription.</param>
        /// <param name="file">The contract file to be uploaded as part of the renewal.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{key}/subscriptions"), SensitiveData]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateClientSubscriptionAsync(string key, [FromForm] DateTime expirationDate, IFormFile file, CancellationToken cancellationToken)
        {
            await _client.CreateSubscription(key, expirationDate, file, cancellationToken);
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
        public async Task<IActionResult> GetClientSubscriptionContractAsync(string key, long id, CancellationToken cancellationToken)
        {
            FileDto file = await _client.GetSubscriptionContractAsync(key, id, cancellationToken);
            return File(file.Content, MediaTypeNames.Application.Pdf, file.Name);
        }
    }
}