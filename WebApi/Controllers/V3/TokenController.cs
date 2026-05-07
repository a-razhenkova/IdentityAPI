using Application;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V3
{
    /// <summary>
    /// Controller responsible for handling authentication-related endpoints.
    /// </summary>
    [Authorize]
    [ApiController, Route("api/v3/[controller]")]
    public class TokenController : JsonApiControllerBase
    {
        private readonly IToken _token;
        private readonly IMapper _mapper;

        public TokenController(IToken token, IMapper mapper)
        {
            _mapper = mapper;
            _token = token;
        }

        /// <summary>
        /// Creates access and refresh tokens for a user by the provided OTP.
        /// </summary>
        /// <param name="userCredentials">The user's OTP and related two-factor authentication information.</param>
        [AllowAnonymous]
        [HttpPost, SensitiveData(IsResponseSensitive = true)]
        [ProducesResponseType(typeof(V1.TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync(V2.UserCredentialsModel userCredentials)
        {
            TokenDto token = await _token.CreateAccessTokenByOtpAsync(userCredentials.UserId, userCredentials.OneTimePassword);
            return Ok(_mapper.Map<V1.TokenModel>(token));
        }
    }
}