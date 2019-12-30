using System.Net.Http;
using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface ISwaggerService
    {
        HttpResponseMessage GetSwaggerJson(Assembly fromAssembly, string documentName);
    }
}