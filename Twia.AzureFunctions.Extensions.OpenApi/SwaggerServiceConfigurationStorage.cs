using System.Reflection;

namespace Twia.Extensions.Swagger
{
    class SwaggerServiceConfigurationStorage : ISwaggerServiceConfigurationStorage
    {
        public Assembly FunctionAssembly { get; set; }
    }
}