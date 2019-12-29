using Microsoft.OpenApi;

namespace Twia.Extensions.Swagger.Config
{
    public class OpenAPiDocumentation
    {
        public string[] XmlDocPaths { get; set; }

        public OpenApiSpecVersion OpenApiSpecVersion { get; set; } = OpenApiSpecVersion.OpenApi3_0;

        public OpenAPiDocumentationDocument[] Documents { get; set; } = { };
    }
}