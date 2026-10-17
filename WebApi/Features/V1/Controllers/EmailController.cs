using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V1
{
    [AuthorizeUser(UserRoles.Administrator)]
    [Route("api/v1/[controller]")]
    public class EmailController : JsonApiControllerBase
    {
        private readonly IEmail _email;

        public EmailController(IEmail email)
        {
            _email = email;
        }

        /// <summary>
        /// Verifies a user's email using the provided verification token.
        /// </summary>
        /// <param name="token">The email verification.</param>
        [HttpPost("verification/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmailVerificationTokenAsync(string token)
        {
            await _email.VerifyToken(token);
            return Ok();
        }
    }
}