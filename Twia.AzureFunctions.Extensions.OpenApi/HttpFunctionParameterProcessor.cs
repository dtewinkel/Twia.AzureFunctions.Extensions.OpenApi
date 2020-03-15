using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionParameterProcessor : IHttpFunctionParameterProcessor
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

        private static readonly string[] _untypedTriggerParameters =
        {
            typeof(HttpRequestMessage).FullName,
            typeof(HttpRequest).FullName
        };

        public HttpFunctionParameterProcessor(IModelMetadataProvider modelMetadataProvider)
        {
            EnsureArg.IsNotNull(modelMetadataProvider, nameof(modelMetadataProvider));

            _modelMetadataProvider = modelMetadataProvider;
        }

        public IList<ApiParameterDescription> GetApiParameterDescriptions(MethodInfo functionMethod, string route)
        {
            EnsureArg.IsNotNull(functionMethod, nameof(functionMethod));

            return ProcessParameterTypes(functionMethod, route ?? "").ToList();
        }

        private IEnumerable<ApiParameterDescription> ProcessParameterTypes(MethodInfo functionMethod, string route)
        {
            foreach (var bodyParameter in GetBodyParameters(functionMethod))
            {
                yield return bodyParameter;
            }

            foreach (var pathParameter in GetPathParameters(functionMethod, route))
            {
                yield return pathParameter;
            }
        }

        private IEnumerable<ApiParameterDescription> GetPathParameters(MethodInfo functionMethod, string route)
        {
            return functionMethod.GetParameters()
                .Where(parameterInfo => Regex.IsMatch(route, $"\\{{{parameterInfo.Name}(:[^?}}]*)?\\??\\}}"))
                .Select(parameterInfo => CreateApiParameterDescription(parameterInfo, BindingSource.Path));
        }

        private IEnumerable<ApiParameterDescription> GetBodyParameters(MethodInfo functionMethod)
        {
            var triggerParameter = functionMethod
                .GetParameters()
                .First(parameter => parameter.GetCustomAttributes(typeof(HttpTriggerAttribute), false).Any());

            // First try OpenApiBodyTypeAttribute, regardless of the parameter's own type.
            var fromBodyAttribute = triggerParameter.GetCustomAttributes(typeof(OpenApiBodyTypeAttribute))
                .Cast<OpenApiBodyTypeAttribute>().FirstOrDefault();
            if (fromBodyAttribute != null)
            {
                yield return CreateApiParameterDescription(triggerParameter.Name, fromBodyAttribute.Type, BindingSource.Body);
            }

            var isTyped = !_untypedTriggerParameters.Contains(triggerParameter.ParameterType.FullName);
            if (isTyped)
            {
                yield return CreateApiParameterDescription(triggerParameter, BindingSource.Body);
            }
        }

        private ApiParameterDescription CreateApiParameterDescription(ParameterInfo parameter, BindingSource bindingSource)
        {
            var name = parameter.Name;
            var type = parameter.ParameterType;

            return CreateApiParameterDescription(name, type, bindingSource);
        }

        private ApiParameterDescription CreateApiParameterDescription(string name, Type type, BindingSource bindingSource)
        {
            var description = new ApiParameterDescription
            {
                Name = name,
                Type = type,
                Source = bindingSource,
                ModelMetadata = _modelMetadataProvider.GetMetadataForType(type),
                ParameterDescriptor = new ParameterDescriptor
                {
                    Name = name,
                    ParameterType = type
                }
            };
            return description;
        }
    }
}