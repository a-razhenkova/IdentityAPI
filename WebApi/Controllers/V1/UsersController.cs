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
        /// <param name="searchParams">Search parameters for filtering users.</param>
        /// <returns>A paginated report of users matching the search criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedReport<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUsersAsync([FromQuery] UserSearchParams searchParams, CancellationToken cancellationToken)
        {
            PaginatedReport<UserDto> searchResult = await _user.SearchAsync(searchParams, cancellationToken);
            return Ok(_mapper.Map<PaginatedReport<UserModel>>(searchResult));
        }

        /// <summary>
        /// Retrieves a user.
        /// </summary>
        /// <param name="id">The external ID of the user to be retrieved.</param>
        /// <returns>The user details if found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(string id)
        {
            UserDto user = await _user.GetAsync(id);
            return Ok(_mapper.Map<UserModel>(user));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="requestModel">The model containing user registration details.</param>
        /// <returns>The external ID of the registered user.</returns>
        [HttpPost]
        [SensitiveData]
        [ProducesResponseType(typeof(SimpleResponseModel<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUserAsync(UserRegistrationModel requestModel)
        {
            string userPublicId = await _user.RegisterAsync(_mapper.Map<UserDto>(requestModel));
            return Created(string.Empty, new SimpleResponseModel<string>(userPublicId));
        }

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The external ID of the user to be updated.</param>  
        /// <param name="requestModel">The model containing the updated user details.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(string id, UserUpdateModel requestModel)
        {
            await _user.UpdateAsync(id, _mapper.Map<UserDto>(requestModel));
            return Ok();
        }

        /// <summary>  
        /// Deletes a user.  
        /// </summary>  
        /// <param name="id">The external ID of the user to be deleted.</param>  
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            await _user.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Changes the password of an existing user.
        /// </summary>
        /// <param name="id">The external ID of the user whose password is to be changed.</param>
        /// <param name="requestModel">The model containing the old and new passwords.</param>
        [HttpPatch("{id}/password"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeUserPasswordAsync(string id, UserPasswordModel requestModel)
        {
            await _user.ChangePasswordAsync(id, requestModel.OldPassword, requestModel.NewPassword);
            return Ok();
        }

        /// <summary>
        /// Changes the email address of an existing user.
        /// </summary>
        /// <param name="id">The external ID of the user whose email is to be changed.</param>
        /// <param name="requestModel">The model containing the new email and the user's password.</param>
        [HttpPatch("{id}/email"), SensitiveData]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeEmailAsync(string id, UserEmailModel requestModel)
        {
            await _user.ChangeEmailAsync(id, requestModel.Email, requestModel.Password);
            return Ok();
        }

        /// <summary>
        /// Sends a verification email to the specified user.
        /// </summary>
        /// <param name="id">The external ID of the user to whom the verification email will be sent.</param>
        [HttpPost("{id}/email/verification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendUserEmailVerificationAsync(string id)
        {
            // TODO: email verification
            return Ok();
        }
    }
}