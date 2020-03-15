using System.Reflection;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class OpenApiServiceConfigurationStorage : IOpenApiServiceConfigurationStorage
    {
        public OpenApiServiceConfigurationStorage(Assembly functionAssembly)
        {
            EnsureArg.IsNotNull(functionAssembly, nameof(functionAssembly));

            FunctionAssembly = functionAssembly;
        }

        public Assembly FunctionAssembly { get; }
    }
}