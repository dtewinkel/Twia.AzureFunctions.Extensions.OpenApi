using System.Text.RegularExpressions;
using EnsureThat;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class SwaggerService : ISwaggerService
    {
        private readonly ISwaggerProvider _swaggerProvider;

        public SwaggerService(ISwaggerProvider swaggerProvider)
        {
            EnsureArg.IsNotNull(swaggerProvider, nameof(swaggerProvider));

            _swaggerProvider = swaggerProvider;
        }

        public string GetSwaggerJson(string documentName, string host = null, string basePath = null, OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0)
        {
            EnsureArg.IsNotNullOrWhiteSpace(documentName, nameof(documentName));

            var (cleanHost, cleanBasePath) = CleanHostAndBasePath(host, basePath);

            var document = _swaggerProvider.GetSwagger(documentName, cleanHost, cleanBasePath);
            return document.SerializeAsJson(openApiSpecVersion);
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