using System.Net.Http;
using System.Reflection;

namespace Twia.Extensions.Swagger
{
    public interface ISwaggerService
    {
        HttpResponseMessage GetSwaggerJson(Assembly fromAssembly, string documentName);
    }
}