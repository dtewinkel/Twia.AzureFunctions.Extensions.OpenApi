using System.Reflection;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class SwaggerServiceConfigurationStorage : ISwaggerServiceConfigurationStorage
    {
        public SwaggerServiceConfigurationStorage(Assembly functionAssembly)
        {
            EnsureArg.IsNotNull(functionAssembly, nameof(functionAssembly));

            FunctionAssembly = functionAssembly;
        }

        public Assembly FunctionAssembly { get; }
    }
}