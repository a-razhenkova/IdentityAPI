using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace WebApi
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class JsonApiControllerBase : ApiControllerBase
    {

    }

    [Authorize]
    [ApiController]
    [ProducesResponseType(typeof(V1.ExceptionModel), StatusCodes.Status500InternalServerError)]
    public class ApiControllerBase : ControllerBase
    {

    }
}