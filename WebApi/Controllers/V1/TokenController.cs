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
        [AllowAnonymous]
        [HttpPost, SensitiveData(IsResponseSensitive = true)]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync()
        {
            var authorization = new Authorization(HttpContext.GetAuthorization());

            TokenDto token = await _token.CreateAccessTokenAsync(authorization);

            return Ok(_mapper.Map<TokenModel>(token));
        }

        /// <summary>
        /// Validates the provided access token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("status"), SkipLog]  
        [ProducesResponseType(typeof(TokenValidationResultModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateAccessTokenAsync()
        {
            TokenValidationResult tokenValidationResult = await _token.ValidateAccessTokenAsync();
            return Ok(_mapper.Map<TokenValidationResultModel>(tokenValidationResult));
        }
    }
}