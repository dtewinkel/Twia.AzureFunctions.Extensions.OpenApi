using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionQueryParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.MethodInfo;
            var ignoredQueryParameters = GetIgnoredQueryParameters(method);
            var queryParameterAttributes = GetQueryParameterAttributes(method, ignoredQueryParameters);
            foreach (var queryParameterAttribute in queryParameterAttributes)
            {
                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = queryParameterAttribute.Name,
                        Schema = context.SchemaGenerator.GenerateSchema(queryParameterAttribute.Type, context.SchemaRepository),
                        Description = queryParameterAttribute.Description,
                        Required = queryParameterAttribute.IsRequired
                    }
                );
            }
        }

        private static List<QueryParameterAttribute> GetQueryParameterAttributes(MethodInfo method, List<string> ignoredQueryParameters)
        {
            var queryParameterAttributes = new List<QueryParameterAttribute>();

            queryParameterAttributes.AddRange(method.GetAttributes<QueryParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));
            queryParameterAttributes.AddRange(method.DeclaringType.GetAttributes<QueryParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));
            queryParameterAttributes.AddRange(method.DeclaringType.Assembly.GetAttributes<QueryParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));

            return queryParameterAttributes;
        }

        private static List<string> GetIgnoredQueryParameters(MethodInfo method)
        {
            var ignoredQueryParameters = new List<string>();
            ignoredQueryParameters.AddRange(method
                .GetAttributes<IgnoreQueryParameterAttribute>()
                .Select(attribute => attribute.Name));
            ignoredQueryParameters.AddRange(method.DeclaringType
                .GetAttributes<IgnoreQueryParameterAttribute>()
                .Select(attribute => attribute.Name));
            return ignoredQueryParameters;
        }
    }
}