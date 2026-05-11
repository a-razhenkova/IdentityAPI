using Application;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V2
{
    /// <summary>
    /// Controller responsible for handling authentication-related endpoints.
    /// </summary>
    [Authorize]
    [ApiController, Route("api/v2/[controller]")]
    public class TokenController : JsonApiControllerBase
    {
        private readonly IToken _token;
        private readonly IMapper _mapper;

        public TokenController(IToken token, IMapper mapper)
        {
            _token = token;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates access and refresh tokens for a users.
        /// </summary>
        /// <param name="userCredentials">User authentication credentials.</param>
        [AllowAnonymous]
        [HttpPost, SensitiveData(IsResponseSensitive = true)]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync(V1.CreateOtpRequest userCredentials)
        {
            TokenDto token = await _token.CreateAccessTokenAsync(userCredentials.Username, userCredentials.Password);

            var response = _mapper.Map<TokenResponse>(token);
            return Ok(response);
        }

        /// <summary>
        /// Refreshes the access token for a user.
        /// </summary>
        [AllowAnonymous]
        [HttpPut, SensitiveData(IsResponseSensitive = true)]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAccessTokenAsync()
        {
            TokenDto token = await _token.RefreshAccessTokenAsync();

            var response = _mapper.Map<TokenResponse>(token);
            return Ok(response);
        }
    }
}