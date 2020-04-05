using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for simple synchronous HTTP functions.
    /// </summary>
    public class ExampleResponseAttributes
    {
        /// <summary>
        /// Function implementing the GET method and a void return type.
        /// </summary>
        /// <param name="req"></param>
        /// <response code="204">No data in body.</response>
        [ProducesResponseType(typeof(void), 204)]
        [FunctionName(nameof(GetVoidProducesResponseType))]
        public void GetVoidProducesResponseType(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to return here.
        }

        /// <summary>
        /// Function implementing the GET method and returning an IActionResult.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An IActionResult.</returns>
        /// <response code="200">An ExampleResponse object.</response>
        [ProducesResponseType(typeof(ExampleResponse), 200)]
        [FunctionName(nameof(GetExampleResponseProducesResponseType))]
        public IActionResult GetExampleResponseProducesResponseType(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new OkResult();
        }

        /// <summary>
        /// Function implementing the GET method and returning an IActionResult.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An IActionResult.</returns>
        /// <response code="200">An ExampleResponse object.</response>
        /// <response code="204">No data to return.</response>
        /// <response code="500">A ClientErrorData describing the error.</response>
        [ProducesResponseType(typeof(ExampleResponse), 200)]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(SerializableError), 400)]
        [FunctionName(nameof(GetMultipleProducesResponseTypes))]
        public IActionResult GetMultipleProducesResponseTypes(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new OkResult();
        }
    }
}
