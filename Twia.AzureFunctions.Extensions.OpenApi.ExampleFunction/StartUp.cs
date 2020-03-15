using System;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Twia.AzureFunctions.Extensions.OpenApi.DependencyInjection;
using Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

[assembly: FunctionsStartup(typeof(StartUp))]

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var functionAssembly = Assembly.GetExecutingAssembly();
            var services = builder.Services;

            services.AddOpenApiService(functionAssembly, options => ConfigureSwaggerOptions(options, functionAssembly));
        }

        private static void ConfigureSwaggerOptions(SwaggerGenOptions options, Assembly functionAssembly)
        {
            foreach (var xmlDocFilePath in functionAssembly.GetXmlFilePaths())
            {
                options.IncludeXmlComments(xmlDocFilePath);
            }

            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Description = "Some great version of the documentation",
                    Title = "Example Function documentation"
                });

#pragma warning disable S1075 // URIs should not be hardcoded
            options.SwaggerDoc("v2",
                new OpenApiInfo
                {
                    Version = "v2",
                    Description = "Some next great version of the documentation",
                    Title = "Example Function documentation version 2",
                    Contact = new OpenApiContact
                    {
                        Email = "john.doe@example.com",
                        Name = "John Doe",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://example.com/license")
                    },
                    TermsOfService = new Uri("https://example.com/terms-of-service")
                });
#pragma warning restore S1075 // URIs should not be hardcoded
        }
    }
}