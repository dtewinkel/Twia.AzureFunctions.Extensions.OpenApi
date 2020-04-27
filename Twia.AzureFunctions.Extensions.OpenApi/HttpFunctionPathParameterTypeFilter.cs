using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionPathParameterTypeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var operationParameter in operation.Parameters)
            {
                if (operationParameter.In == ParameterLocation.Path)
                {
                    var description = context.ApiDescription.ParameterDescriptions.FirstOrDefault(parameter => parameter.Name == operationParameter.Name);
                    operationParameter.Schema = context.SchemaGenerator.GenerateSchema(description?.Type, context.SchemaRepository);
                }
            }
        }
    }

}