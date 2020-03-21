using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
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
        private static readonly string[] _defaultFormats = {"application/json"};
        private readonly IHttpFunctionParameterProcessor _httpFunctionParameterProcessor;
        private readonly IHttpFunctionResponseProcessor _httpFunctionResponseProcessor;
        private readonly string _routePrefix;

        public HttpFunctionProcessor(
            IHttpFunctionParameterProcessor httpFunctionParameterProcessor,
            IHttpFunctionResponseProcessor httpFunctionResponseProcessor,
            IOptions<HttpOptions> httpOptions)
        {
            EnsureArg.IsNotNull(httpFunctionParameterProcessor, nameof(httpFunctionParameterProcessor));
            EnsureArg.IsNotNull(httpFunctionResponseProcessor, nameof(httpFunctionResponseProcessor));
            EnsureArg.IsNotNull(httpOptions, nameof(httpOptions));

            _httpFunctionParameterProcessor = httpFunctionParameterProcessor;
            _httpFunctionResponseProcessor = httpFunctionResponseProcessor;
            _routePrefix = httpOptions.Value.RoutePrefix;
        }

        public ApiDescriptionGroup ProcessHttpFunction(MethodInfo httpFunction)
        {
            EnsureArg.IsNotNull(httpFunction, nameof(httpFunction));

            var functionName = GetFunctionName(httpFunction);
            var httpTriggerAttribute = GetHttpTriggerAttribute(httpFunction);
            var methods = GetMethods(httpTriggerAttribute);
            var groupName = GetInformationFromApiExplorerSettingsAttribute(httpFunction);
            var parameters = _httpFunctionParameterProcessor
                .GetApiParameterDescriptions(httpFunction, httpTriggerAttribute.Route)
                .ToList();

            var responseTypes = _httpFunctionResponseProcessor.GetResponseTypes(httpFunction);
            var apiDescriptions = new List<ApiDescription>();
            var route = GetRoute(httpTriggerAttribute.Route, functionName);
            foreach (var method in methods)
            {
                var description = CreateApiDescription(httpFunction, method, route, functionName, groupName, parameters);
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



        private static IList<string> GetMethods(HttpTriggerAttribute httpTriggerAttribute)
        {
            return (httpTriggerAttribute.Methods ?? _defaultMethods)
                .Select(m => m.Trim().ToUpperInvariant())
                .ToList();
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

        private ApiDescription CreateApiDescription(MethodInfo httpFunctionMethod, string method, string route,
            string functionName, string groupName, IList<ApiParameterDescription> parameters)
        {
            var parameterDescriptors = parameters
                .Select(apiParameterDescription => apiParameterDescription.ParameterDescriptor)
                .ToList();

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
                    Parameters = parameterDescriptors,
                    RouteValues = new Dictionary<string, string>
                    {
                        {"controller", functionName},
                        {"action", method}
                    },
                    ActionName = functionName
                },
            };

            foreach (var format in _defaultFormats)
            {
                description.SupportedRequestFormats.Add(
                    new ApiRequestFormat
                    {
                        MediaType = format
                    }
                );
            }

            foreach (var apiParameterDescription in parameters)
            {
                description.ParameterDescriptions.Add(apiParameterDescription);
            }

            return description;
        }

        private string GetRoute(string rawRoute, string functionName)
        {
            if (string.IsNullOrWhiteSpace(rawRoute))
            {
                rawRoute = functionName;
            }

            var cleanRoute = Regex.Replace(rawRoute, @"\{(?<name>[^?:}]*)(:[^?}]*)?\??\}", @"{${name}}");

            return $"{_routePrefix}/{cleanRoute}".TrimEnd('/');
        }
    }
}