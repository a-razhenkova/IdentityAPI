using Microsoft.OpenApi.Models;
using Shared;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi
{
    public class SwaggerCorrelationIdFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = HttpHeaders.CorrelationId,
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}