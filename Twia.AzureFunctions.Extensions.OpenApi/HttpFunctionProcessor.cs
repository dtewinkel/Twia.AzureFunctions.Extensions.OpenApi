using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
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
        private static readonly string[] _defaultMethods = {"get", "post", "put", "delete", "head", "patch", "options"};
        private readonly IHttpFunctionResponseProcessor _httpFunctionResponseProcessor;
        private readonly string _routePrefix;

        public HttpFunctionProcessor(
            IHttpFunctionResponseProcessor httpFunctionResponseProcessor,
            IOptions<HttpOptions> httpOptions)
        {
            EnsureArg.IsNotNull(httpFunctionResponseProcessor, nameof(httpFunctionResponseProcessor));
            EnsureArg.IsNotNull(httpOptions, nameof(httpOptions));

            _httpFunctionResponseProcessor = httpFunctionResponseProcessor;
            _routePrefix = httpOptions.Value.RoutePrefix;
        }

        public ApiDescriptionGroup ProcessHttpFunction(MethodInfo httpFunctionMethod)
        {
            EnsureArg.IsNotNull(httpFunctionMethod, nameof(httpFunctionMethod));

            var functionName = GetFunctionName(httpFunctionMethod);
            var httpTriggerAttribute = GetHttpTriggerAttribute(httpFunctionMethod);
            var route = GetRoute(httpTriggerAttribute, functionName);
            var methods = GetMethods(httpTriggerAttribute);
            var groupName = GetInformationFromApiExplorerSettingsAttribute(httpFunctionMethod);

            var responseTypes = _httpFunctionResponseProcessor.GetResponseTypes(httpFunctionMethod);
            var apiDescriptions = new List<ApiDescription>();
            foreach (var method in methods)
            {
                var description = CreateApiDescription(httpFunctionMethod, method, route, functionName, groupName);
                AddResponseTypes(description, responseTypes);
                apiDescriptions.Add(description);
            }

            return new ApiDescriptionGroup(functionName, apiDescriptions);
        }

        private static string GetInformationFromApiExplorerSettingsAttribute(MethodInfo httpFunctionMethod)
        {
            return httpFunctionMethod.GetCustomAttributes(typeof(ApiExplorerSettingsAttribute))
                .Cast<ApiExplorerSettingsAttribute>().SingleOrDefault()?.GroupName;
        }

        private static HttpTriggerAttribute GetHttpTriggerAttribute(MethodInfo httpFunctionMethod)
        {
            return httpFunctionMethod
                .GetParameters()
                .Single(p => p.GetCustomAttributes(typeof(HttpTriggerAttribute)).Any())
                .GetCustomAttributes(typeof(HttpTriggerAttribute))
                .Cast<HttpTriggerAttribute>()
                .Single();
        }



        private static IEnumerable<string> GetMethods(HttpTriggerAttribute httpTriggerAttribute)
        {
            return (httpTriggerAttribute.Methods ?? _defaultMethods)
                .Select(m => m.Trim().ToUpperInvariant());
        }

        private static string GetFunctionName(MethodInfo httpFunctionMethod)
        {
            return httpFunctionMethod.GetCustomAttributes(typeof(FunctionNameAttribute))
                .Cast<FunctionNameAttribute>().Single().Name;
        }

        private static void AddResponseTypes(ApiDescription description, IReadOnlyList<ApiResponseType> responseTypes)
        {
            foreach (var responseType in responseTypes)
            {
                description.SupportedResponseTypes.Add(responseType);
            }
        }

        private static ApiDescription CreateApiDescription(MethodInfo httpFunctionMethod, string method, string route,
            string functionName, string groupName)
        {
            var description = new ApiDescription
            {
                HttpMethod = method,
                RelativePath = route,
                GroupName = groupName,
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
            return description;
        }

        private string GetRoute(HttpTriggerAttribute httpTriggerAttribute, string functionName)
        {
            var route = httpTriggerAttribute.Route;
            if (string.IsNullOrWhiteSpace(route))
            {
                route = functionName;
            }

            return $"{_routePrefix}/{route}"
                .Replace("?", "")
                .TrimEnd('/');
        }
    }
}