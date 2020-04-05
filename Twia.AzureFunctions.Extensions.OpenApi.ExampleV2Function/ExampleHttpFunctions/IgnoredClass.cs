using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Functions in this class are HTTP triggered functions that are ignored.
    /// </summary>
    [OpenApiIgnore]
    public class IgnoredClass
    {
        /// <summary>
        /// Ignored Function based on ignored class.
        /// </summary>
        /// <remarks>
        /// This function is ignored due to the [OpenApiIgnore] attribute on it's class.
        /// </remarks>
        /// <param name="req">The request details.</param>
        [FunctionName(nameof(IgnoreClassWithOpenApiIgnore))]
        public void IgnoreClassWithOpenApiIgnore(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to be done here.
        }
    }
}
