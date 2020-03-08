using System;
using System.Reflection;
using EnsureThat;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services, Assembly functionAssembly, Action<SwaggerGenOptions> setupSwaggerGen = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            EnsureArg.IsNotNull(functionAssembly, nameof(functionAssembly));

            services.AddScoped<ISwaggerService, SwaggerService>();
            services.AddSingleton<IHttpFunctionProcessor, HttpFunctionProcessor>();
            services.AddSingleton<IHttpFunctionParameterProcessor, HttpFunctionParameterProcessor>();
            services.AddSingleton<IHttpFunctionResponseProcessor, HttpFunctionResponseProcessor>();
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, FunctionApiDescriptionGroupCollectionProvider>();
            services.AddSingleton<ISwaggerServiceConfigurationStorage>(new SwaggerServiceConfigurationStorage(functionAssembly));

            services.AddSwaggerGen(setupSwaggerGen);

            return services;
        }
    }
}