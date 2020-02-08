using Microsoft.OpenApi;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface ISwaggerService
    {
        string GetSwaggerJson(string documentName, string host = null, string basePath = null, OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0);

    }
}