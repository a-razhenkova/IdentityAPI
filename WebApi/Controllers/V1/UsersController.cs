using Application;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V1
{
    [AuthorizeUser(UserRoles.Administrator)]
    [Route("api/v1/[controller]")]
    public class UsersController : JsonApiControllerBase
    {
        private readonly IUser _user;
        private readonly IMapper _mapper;

        public UsersController(IUser user, IMapper mapper)
        {
            _mapper = mapper;
            _user = user;
        }

        /// <summary>
        /// Retrieves list of users.
        /// </summary>
        /// <param name="request">Search parameters for filtering users.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated report of users matching the search criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedReportResponse<UserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUsersAsync([FromQuery] SearchUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<SearchUserQuery>(request);
            PaginatedReportDto<UserDto> searchResult = await _user.SearchAsync(command, cancellationToken);

            var response = _mapper.Map <PaginatedReportResponse<UserResponse>>(searchResult);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a user.
        /// </summary>
        /// <param name="id">The external ID of the user to be retrieved.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details if found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(string id, CancellationToken cancellationToken)
        {
            UserDto user = await _user.GetAsync(id, cancellationToken);
            return Ok(_mapper.Map<UserResponse>(user));
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">Contains user registration details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The external ID of the registered user.</returns>
        [HttpPost]
        [SensitiveData]
        [ProducesResponseType(typeof(SimpleResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateUserCommand>(request);
            string userPublicId = await _user.CreateAsync(command, cancellationToken);

            var response = new SimpleResponse<string>(userPublicId);
            return Created(string.Empty, response);
        }

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The external ID of the user to be updated.</param>
        /// <param name="request">Contains the updated user details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateUserCommand>(request);
            await _user.UpdateAsync(id, command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The external ID of the user to be deleted.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            await _user.DeleteAsync(id, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Changes the password of an existing user.
        /// </summary>
        /// <param name="id">The external ID of the user whose password is to be changed.</param>
        /// <param name="request">Contains the old and new passwords.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPatch("{id}/password"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserPasswordAsync(string id, UpdateUserPasswordRequest request, CancellationToken cancellationToken)
        {
            await _user.UpdatePasswordAsync(id, request.OldPassword, request.NewPassword, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Changes the email address of an existing user.
        /// </summary>
        /// <param name="id">The external ID of the user whose email is to be changed.</param>
        /// <param name="request">Contains the new email and the user's password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPatch("{id}/email"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEmailAsync(string id, UpdateUserEmailRequest request, CancellationToken cancellationToken)
        {
            await _user.UpdateEmailAsync(id, request.Email, request.Password, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Creates and sends a verification email to the specified user.
        /// </summary>
        /// <param name="id">The external ID of the user to whom the verification email will be sent.</param>
        [HttpPost("{id}/email/verification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAndSendUserEmailVerificationAsync(string id)
        {
            await _user.CreateAndSendEmailVerificationAsync(id);
            return Ok();
        }
    }
}