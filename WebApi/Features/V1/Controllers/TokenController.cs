using Application;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace WebApi.V1
{
    [Route("api/v1/[controller]")]
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
        /// Creates an access token for clients.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [AllowAnonymous]
        [HttpPost, SensitiveData(IsResponseSensitive = true)]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync(CancellationToken cancellationToken)
        {
            var authorization = new Authorization(HttpContext.GetAuthorization());
            TokenDto token = await _token.CreateAccessTokenAsync(authorization, cancellationToken);

            var response = _mapper.Map<TokenResponse>(token);
            return Ok(response);
        }

        /// <summary>
        /// Validates the provided access token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("status"), SkipLog]
        [ProducesResponseType(typeof(TokenValidationResultResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateAccessTokenAsync()
        {
            TokenValidationResult tokenValidationResult = await _token.ValidateAccessTokenAsync();

            var response = _mapper.Map<TokenValidationResultResponse>(tokenValidationResult);
            return Ok(response);
        }
    }
}