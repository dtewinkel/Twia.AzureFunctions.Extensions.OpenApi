using System.Reflection;

namespace Twia.Extensions.Swagger
{
    public interface ISwaggerServiceConfigurationStorage
    {
        Assembly FunctionAssembly { get; }
    }
}