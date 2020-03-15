using Microsoft.OpenApi.Models;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface IOpenApiService
    {
        OpenApiDocument GetOpenApiDocument(string documentName, string host = null, string basePath = null);
    }
}