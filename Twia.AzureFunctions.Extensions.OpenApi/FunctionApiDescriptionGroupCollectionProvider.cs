﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.WebJobs;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class FunctionApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly IOpenApiServiceConfigurationStorage _configuration;
        private readonly IHttpFunctionProcessor _httpFunctionMethodProcessor;

        public FunctionApiDescriptionGroupCollectionProvider(
            IOpenApiServiceConfigurationStorage configuration,
            IHttpFunctionProcessor httpFunctionMethodProcessor)
        {
            EnsureArg.IsNotNull(configuration, nameof(configuration));
            EnsureArg.IsNotNull(httpFunctionMethodProcessor, nameof(httpFunctionMethodProcessor));

            _configuration = configuration;
            _httpFunctionMethodProcessor = httpFunctionMethodProcessor;
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups => GetFunctionApiDescriptors();

        private ApiDescriptionGroupCollection GetFunctionApiDescriptors()
        {
            var apiDescriptionGroups =
                GetHttpFunctionMethods()
                    .Select(_httpFunctionMethodProcessor.ProcessHttpFunction)
                    .ToList();
            return new ApiDescriptionGroupCollection(apiDescriptionGroups, 1);
        }

        private IEnumerable<MethodInfo> GetHttpFunctionMethods()
        {
            return _configuration.FunctionAssembly.GetTypes()
                .Where(t => !t.HasAttribute<OpenApiIgnoreAttribute>())
                .SelectMany(t => t.GetMethods())
                .Where(m => m.HasAttribute<FunctionNameAttribute>())
                .Where(m => m.GetParameters().Any(p => p.HasAttribute<HttpTriggerAttribute>()))
                .Where(m => !m.HasAttribute<OpenApiIgnoreAttribute>())
                .Where(m => !m.GetAttributes<ApiExplorerSettingsAttribute>().Any(a => a.IgnoreApi));
        }
    }
}

