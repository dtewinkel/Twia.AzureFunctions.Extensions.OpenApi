using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface ISwaggerServiceConfigurationStorage
    {
        Assembly FunctionAssembly { get; }
    }
}