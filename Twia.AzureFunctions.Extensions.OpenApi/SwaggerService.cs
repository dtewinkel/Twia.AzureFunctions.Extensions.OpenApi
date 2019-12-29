using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Twia.Extensions.Swagger.Config;

namespace Twia.Extensions.Swagger
{
    public class SwaggerService : ISwaggerService
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly OpenApiSpecVersion _openApiSpecVersion;

        public SwaggerService(ISwaggerProvider swaggerProvider, IOptions<OpenAPiDocumentation> options)
        {
            _swaggerProvider = swaggerProvider;
            _openApiSpecVersion = options.Value.OpenApiSpecVersion;
        }

        public HttpResponseMessage GetSwaggerJson(Assembly fromAssembly, string documentName)
        {
            var document = _swaggerProvider.GetSwagger(documentName ?? "v1");
            var documentJson = document.SerializeAsJson(_openApiSpecVersion);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(documentJson, Encoding.UTF8, "application/json")
            };
        }
    }
}