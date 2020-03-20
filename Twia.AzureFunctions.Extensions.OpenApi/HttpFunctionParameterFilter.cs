using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            FixRequiredParameters(operation, context);
        }

        private void FixRequiredParameters(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var operationParameter in operation.Parameters)
            {
                var description = context.ApiDescription.ParameterDescriptions.FirstOrDefault(parameter => parameter.Name == operationParameter.Name);
                operationParameter.Required = !(description?.RouteInfo.IsOptional ?? false);
            }
        }
    }
}