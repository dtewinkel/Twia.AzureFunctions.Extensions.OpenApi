using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for simple synchronous HTTP functions.
    /// </summary>
    public class ExampleReturnTypes
    {
        /// <summary>
        /// Function implementing the GET method and a void return type.
        /// </summary>
        /// <param name="req"></param>
        [FunctionName(nameof(GetVoidReturnValue))]
        public void GetVoidReturnValue(
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
        [FunctionName(nameof(GetIActionResultReturnValue))]
        public IActionResult GetIActionResultReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new OkResult();
        }

        /// <summary>
        /// Function implementing the GET method and returning an HttpResponseMessage.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An HttpResponseMessage.</returns>
        [FunctionName(nameof(GetHttpResponseMessageReturnValue))]
        public HttpResponseMessage GetHttpResponseMessageReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Function implementing the GET method and returning an ExampleResponse.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An ExampleResponse.</returns>
        [FunctionName(nameof(GetExampleResponseReturnValue))]
        public ExampleResponse GetExampleResponseReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new ExampleResponse {Message = "Hi world!"};
        }

        /// <summary>
        /// Function implementing the GET method and returning a collection of ExampleResponses.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An ExampleResponse array.</returns>
        [FunctionName(nameof(GetExampleResponseArrayReturnValue))]
        public IEnumerable<ExampleResponse> GetExampleResponseArrayReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return new [] {new ExampleResponse { Message = "Hi world!" } };
        }
    }
}
