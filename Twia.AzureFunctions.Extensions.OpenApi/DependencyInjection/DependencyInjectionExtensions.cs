﻿using System;
using System.Reflection;
using EnsureThat;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddOpenApiService(this IServiceCollection services, Assembly functionAssembly, Action<SwaggerGenOptions> setupSwaggerGen = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            EnsureArg.IsNotNull(functionAssembly, nameof(functionAssembly));

            services.AddScoped<IOpenApiService, OpenApiService>();
            services.AddSingleton<IHttpFunctionProcessor, HttpFunctionProcessor>();
            services.AddSingleton<IHttpFunctionParameterProcessor, HttpFunctionParameterProcessor>();
            services.AddSingleton<IHttpFunctionResponseProcessor, HttpFunctionResponseProcessor>();
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, FunctionApiDescriptionGroupCollectionProvider>();
            services.AddSingleton<IOpenApiServiceConfigurationStorage>(new OpenApiServiceConfigurationStorage(functionAssembly));

            services.AddSwaggerGen(setupSwaggerGen);

            return services;
        }
    }
}