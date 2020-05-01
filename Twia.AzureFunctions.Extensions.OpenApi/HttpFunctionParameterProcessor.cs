using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
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
                .Select(parameterInfo => CreatePathParameterDescription(parameterInfo,
                    Regex.IsMatch(route, $"\\{{{parameterInfo.Name}(:[^?}}]*)?\\?\\}}")));
        }

        private IEnumerable<ApiParameterDescription> GetBodyParameters(MethodInfo functionMethod)
        {
            var triggerParameter = functionMethod
                .GetParameters()
                .First(parameter => parameter.HasAttribute<HttpTriggerAttribute>());

            // First try OpenApiBodyTypeAttribute, regardless of the parameter's own type.
            var fromBodyAttribute = triggerParameter.GetAttributes<OpenApiBodyTypeAttribute>().FirstOrDefault();
            if (fromBodyAttribute != null)
            {
                yield return CreateBodyParameterDescription(triggerParameter.Name, fromBodyAttribute.Type);
            }

            var isTyped = !_untypedTriggerParameters.Contains(triggerParameter.ParameterType.FullName);
            if (isTyped)
            {
                yield return CreateBodyParameterDescription(triggerParameter.Name, triggerParameter.ParameterType);
            }
        }

        private ApiParameterDescription CreatePathParameterDescription(ParameterInfo parameter, bool isOptional)
        {
            return new ApiParameterDescription
            {
                Name = parameter.Name,
                Type = parameter.ParameterType,
                Source = BindingSource.Path,
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = isOptional
                },
                ParameterDescriptor = new ControllerParameterDescriptor
                {
                    ParameterType = parameter.ParameterType,
                    Name = parameter.Name,
                    ParameterInfo = parameter
                }
            };
        }
        private ApiParameterDescription CreateBodyParameterDescription(string name, Type type)
        {
            return new ApiParameterDescription
            {
                Name = name,
                Type = type,
                Source = BindingSource.Body,
                ModelMetadata = _modelMetadataProvider.GetMetadataForType(type)
            };
        }
    }
}