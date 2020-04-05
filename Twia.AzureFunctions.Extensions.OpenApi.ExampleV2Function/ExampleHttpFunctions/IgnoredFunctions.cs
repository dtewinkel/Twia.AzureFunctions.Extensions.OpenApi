using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Functions in this class are HTTP triggered functions that are ignored.
    /// </summary>
    public class IgnoredFunctions
    {
        /// <summary>
        /// Ignored Function.
        /// </summary>
        /// <remarks>
        /// This function is ignored due to the [ApiExplorerSettings(IgnoreApi = true)] attribute.
        /// </remarks>
        /// <param name="req">The request details.</param>
        /// <returns>An Async action result.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [FunctionName(nameof(IgnoreWithApiExplorerSetting))]
        public IActionResult IgnoreWithApiExplorerSetting(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new OkObjectResult("Hello, I am ignored in the Open API documentation using [ApiExplorerSettings(IgnoreApi = true)]!");
        }

        /// <summary>
        /// Ignored Function.
        /// </summary>
        /// <remarks>
        /// This function is ignored due to the [OpenApiIgnore] attribute.
        /// </remarks>
        /// <param name="req">The request details.</param>
        /// <returns>An Async action result.</returns>
        [OpenApiIgnore]
        [FunctionName(nameof(IgnoreWithOpenApiIgnore))]
        public IActionResult IgnoreWithOpenApiIgnore(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new OkObjectResult("Hello, I am ignored in the Open API documentation using [OpenApiIgnore]!");
        }
    }
}
