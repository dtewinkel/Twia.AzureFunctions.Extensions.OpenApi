using System.Text.RegularExpressions;
using EnsureThat;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class OpenApiService : IOpenApiService
    {
        private readonly ISwaggerProvider _swaggerProvider;

        public OpenApiService(ISwaggerProvider swaggerProvider)
        {
            EnsureArg.IsNotNull(swaggerProvider, nameof(swaggerProvider));

            _swaggerProvider = swaggerProvider;
        }

        public OpenApiDocument GetOpenApiDocument(string documentName, string host = null, string basePath = null)
        {
            EnsureArg.IsNotNullOrWhiteSpace(documentName, nameof(documentName));

            var (cleanHost, cleanBasePath) = CleanHostAndBasePath(host, basePath);

            return _swaggerProvider.GetSwagger(documentName, cleanHost, cleanBasePath);
        }

        private static (string cleanHost, string cleanBasePath) CleanHostAndBasePath(string host, string basePath)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                host = null;
            }
            else
            {
                host = host.Trim(' ').TrimEnd('/');
                if (!Regex.IsMatch(host, "^https?://"))
                {
                    host = $"https://{host}";
                }
            }

            basePath = string.IsNullOrWhiteSpace(basePath) ? null : $"/{basePath.Trim('/', ' ')}";

            return (host, basePath);
        }
    }
}