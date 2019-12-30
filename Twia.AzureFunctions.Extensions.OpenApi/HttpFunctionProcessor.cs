using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionProcessor : IHttpFunctionProcessor
    {
        private readonly IHttpFunctionResponseProcessor _httpFunctionResponseProcessor;
        private readonly string _routePrefix;

        public HttpFunctionProcessor(
            IHttpFunctionResponseProcessor httpFunctionResponseProcessor,
            IOptions<HttpOptions> httpOptions)
        {
            _httpFunctionResponseProcessor = httpFunctionResponseProcessor;
            _routePrefix = httpOptions.Value.RoutePrefix;
        }

        public ApiDescriptionGroup ProcessHttpFunction(MethodInfo httpFunctionMethod)
        {
            var functionAttribute = httpFunctionMethod.GetCustomAttributes(typeof(FunctionNameAttribute))
                .Cast<FunctionNameAttribute>().Single();
            var functionName = functionAttribute.Name;
            var httpTriggerParameter = httpFunctionMethod.GetParameters()
                .Single(p => p.GetCustomAttributes(typeof(HttpTriggerAttribute)).Any());
            var httpTriggerAttribute = httpTriggerParameter.GetCustomAttributes(typeof(HttpTriggerAttribute))
                .Cast<HttpTriggerAttribute>().Single();
            var authLevel = httpTriggerAttribute.AuthLevel;
            var route = GetRoute(httpTriggerAttribute, functionName);
            var methods =
                (httpTriggerAttribute.Methods ?? new[] {"get", "post", "put", "delete", "head", "patch", "options"})
                .Select(m => m.Trim().ToUpperInvariant()).ToArray();

            var apiDescriptions = new List<ApiDescription>();
            foreach (var method in methods)
            {
                var description = new ApiDescription
                {
                    HttpMethod = method,
                    RelativePath = route,
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        DisplayName = functionName,
                        MethodInfo = httpFunctionMethod,
                        ControllerName = method,
                        ControllerTypeInfo = httpFunctionMethod.DeclaringType.GetTypeInfo(),
                        Parameters = new List<ParameterDescriptor>(),
                        RouteValues = new Dictionary<string, string>()
                        {
                            {"controller", functionName},
                            {"action", method}
                        },
                        ActionName = functionName
                    }
                };

                _httpFunctionResponseProcessor.AddResponseTypes(description.SupportedResponseTypes, httpFunctionMethod);
                apiDescriptions.Add(description);
            }

            return new ApiDescriptionGroup(functionName, apiDescriptions);
        }

        private string GetRoute(HttpTriggerAttribute httpTriggerAttribute, string functionName)
        {
            var route = httpTriggerAttribute.Route;
            if (String.IsNullOrWhiteSpace(route))
            {
                route = functionName;
            }

            var routeWithPrefix = $"{_routePrefix}/{route}";
            return routeWithPrefix.TrimEnd('/');
        }
    }
}