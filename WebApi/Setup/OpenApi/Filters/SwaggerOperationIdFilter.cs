using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi
{
    public class SwaggerOperationIdFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.OperationId = context.MethodInfo.Name;

            if (context.ApiDescription.RelativePath?.Contains("api/v1") == true)
            {
                operation.OperationId += "_V1";
            }
            else if (context.ApiDescription.RelativePath?.Contains("api/v2") == true)
            {
                operation.OperationId += "_V2";
            }
            else if (context.ApiDescription.RelativePath?.Contains("api/v3") == true)
            {
                operation.OperationId += "_V3";
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}