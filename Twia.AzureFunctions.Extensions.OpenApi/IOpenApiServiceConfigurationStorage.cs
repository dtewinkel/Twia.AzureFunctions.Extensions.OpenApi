using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface IOpenApiServiceConfigurationStorage
    {
        Assembly FunctionAssembly { get; }
    }
}