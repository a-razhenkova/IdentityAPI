using Microsoft.AspNetCore.Mvc;
using Shared;

namespace WebApi.V1
{
    [AuthorizeUser(UserRoles.Administrator)]
    [Route("api/v1/[controller]")]
    public class EmailController : JsonApiControllerBase
    {
        public EmailController()
        {
        }

        /// <summary>
        /// Verifies a user's email using the provided verification token.
        /// </summary>
        /// <param name="token">The email verification.</param>
        [HttpPost("verification/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyUserEmailVerificationAsync(string token)
        {
            // TODO: email verification
            return Ok();
        }
    }
}