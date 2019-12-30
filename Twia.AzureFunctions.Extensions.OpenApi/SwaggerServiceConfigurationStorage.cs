using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    class SwaggerServiceConfigurationStorage : ISwaggerServiceConfigurationStorage
    {
        public Assembly FunctionAssembly { get; set; }
    }
}