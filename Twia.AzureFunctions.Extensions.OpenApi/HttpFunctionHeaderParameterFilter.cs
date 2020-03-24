using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            EnsureArg.IsNotNull(operation, nameof(operation));
            EnsureArg.IsNotNull(context, nameof(context));

            var method = context.MethodInfo;
            var ignoredHeaderParameters = GetIgnoredHeaderParameters(method);
            var headerParameterAttributes = GetHeaderParameterAttributes(method, ignoredHeaderParameters);
            foreach (var headerParameterAttribute in headerParameterAttributes)
            {
                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        In = ParameterLocation.Header,
                        Name = headerParameterAttribute.Name,
                        Schema = context.SchemaGenerator.GenerateSchema(headerParameterAttribute.Type, context.SchemaRepository, method),
                        Description = headerParameterAttribute.Description,
                        Required = headerParameterAttribute.IsRequired
                    }
                );
            }
        }

        private static IEnumerable<HeaderParameterAttribute> GetHeaderParameterAttributes(MemberInfo method, ICollection<string> ignoredQueryParameters)
        {
            var headerParameterAttributes = new List<HeaderParameterAttribute>();

            headerParameterAttributes.AddRange(method.GetAttributes<HeaderParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));
            headerParameterAttributes.AddRange(method.DeclaringType.GetAttributes<HeaderParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));
            headerParameterAttributes.AddRange(method.DeclaringType.Assembly.GetAttributes<HeaderParameterAttribute>()
                .Where(attribute => !ignoredQueryParameters.Contains(attribute.Name)));

            return headerParameterAttributes;
        }

        private static List<string> GetIgnoredHeaderParameters(MemberInfo method)
        {
            var ignoredHeaderParameters = new List<string>();
            ignoredHeaderParameters.AddRange(method
                .GetAttributes<IgnoreHeaderParameterAttribute>()
                .Select(attribute => attribute.Name));
            ignoredHeaderParameters.AddRange(method.DeclaringType
                .GetAttributes<IgnoreHeaderParameterAttribute>()
                .Select(attribute => attribute.Name));
            return ignoredHeaderParameters;
        }
    }
}