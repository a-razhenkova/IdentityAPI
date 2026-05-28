using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V1
{
    [Route("api/v1/[controller]")]
    public class OtpController : JsonApiControllerBase
    {
        private readonly IOtp _otp;

        public OtpController(IOtp otp)
        {
            _otp = otp;
        }

        /// <summary>
        /// Generates a OTP for the user based on the provided credentials.
        /// </summary>
        /// <param name="request">User authentication credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user's external ID associated with the generated OTP.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(SimpleResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAndSendOtpAsync(TokenRequest request, CancellationToken cancellationToken)
        {
            string userPublicId = await _otp.CreateAndSendAsync(request.Username, request.Password, cancellationToken);

            var response = new SimpleResponse<string>(userPublicId);
            return Ok(response);
        }
    }
}