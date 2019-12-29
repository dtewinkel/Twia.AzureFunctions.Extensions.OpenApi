using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Twia.Extensions.Swagger.Config;

namespace Twia.Extensions.Swagger.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services, Assembly functionAssembly)
        {
            ExtendConfiguration(services);
            services.AddScoped<ISwaggerService, SwaggerService>();
            services.AddSingleton<IHttpFunctionProcessor, HttpFunctionProcessor>();
            services.AddSingleton<IHttpFunctionResponseProcessor, HttpFunctionResponseProcessor>();
            services.AddSingleton<IApiDescriptionGroupCollectionProvider, FunctionApiDescriptionGroupCollectionProvider>();
            services.AddSingleton<ISwaggerServiceConfigurationStorage>(provider => new SwaggerServiceConfigurationStorage { FunctionAssembly = functionAssembly });
            services.AddOptions<OpenAPiDocumentation>()
                .Configure<IConfiguration>((settings, configuration) => { configuration.Bind("OpenAPiDocumentation", settings); });
            var serviceProvider = services.BuildServiceProvider();
            var swaggerConfig = serviceProvider.GetRequiredService<IOptions<OpenAPiDocumentation>>();
            services.AddSwaggerGen(options => ConfigureOptions(options, swaggerConfig.Value, functionAssembly));

            return services;
        }

        private static void ExtendConfiguration(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var originalConfiguration = serviceProvider.GetRequiredService<IConfiguration>();

            var configuration = new ConfigurationBuilder()
                .AddConfiguration(originalConfiguration)
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true, false)
                .Build();

            services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
        }

        private static void ConfigureOptions(SwaggerGenOptions options, OpenAPiDocumentation documentationConfig, Assembly functionAssembly)
        {
            if (documentationConfig.Documents == null || documentationConfig.Documents.Length == 0)
            {
                documentationConfig.Documents = new[] {new OpenAPiDocumentationDocument()};
            }

            var xmlDocPaths = documentationConfig.XmlDocPaths ?? GetXmlLocations(functionAssembly);
            foreach (var xmlDocPath in xmlDocPaths)
            {
                options.IncludeXmlComments(xmlDocPath);
            }

            foreach (var document in documentationConfig.Documents)
            {
                options.SwaggerDoc(document.Name,
                    new OpenApiInfo
                    {
                        Version = document.Version,
                        Description = document.Description,
                        Title = document.Title
                    });
            }
        }

        private static IEnumerable<string> GetXmlLocations(Assembly functionAssembly)
        {
            var inPath = Path.GetDirectoryName(functionAssembly.Location);
            var dllFiles = Directory.EnumerateFiles(inPath, "*.dll");
            var xmlFiles = new List<string>();
            foreach (var dllFile in dllFiles)
            {
                var fileInfo = new FileInfo(dllFile);
                var baseName = Path.GetFileNameWithoutExtension(dllFile);
                var path = Path.GetDirectoryName(dllFile);
                var basePath = Path.Combine(path, baseName);
                var xmlFile = $"{basePath}.xml";
                if (File.Exists(xmlFile))
                {
                    xmlFiles.Add(xmlFile);
                }
                basePath = Path.Combine(Environment.CurrentDirectory, baseName);
                xmlFile = $"{basePath}.xml";
                if (File.Exists(xmlFile))
                {
                    xmlFiles.Add(xmlFile);
                }
            }
            return xmlFiles;
        }
    }
}