using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for asynchronous HTTP functions.
    /// </summary>
    public class ExampleAsyncReturnTypes
    {
        /// <summary>
        /// Async function returning an Task.
        /// </summary>
        /// <remarks>
        /// Status code will be 204 (200 for v1 functions). 
        /// </remarks>
        /// <param name="req"></param>
        [FunctionName(nameof(GetTaskReturnValue))]
        public Task GetTaskReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Async function returning an Task&lt;ExampleResponse&gt;.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An Task&lt;ExampleResponse&gt;.</returns>
        /// <remarks>Status code will be 200 and the returned ty will be ExampleResponse.</remarks>
        [FunctionName(nameof(GetTaskOfExampleResponseReturnValue))]
        public Task<ExampleResponse> GetTaskOfExampleResponseReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return Task.FromResult(new ExampleResponse {Message = "Hello async world!"});
        }

        /// <summary>
        /// Async function implementing the GET method and returning an Task&lt;IActionResult&gt;.
        /// </summary>
        /// <remarks>
        /// Status code will be 200 and return type will be object.
        ///
        /// It is advised to annotate the return type with the `ProducesResponseTypeAttribute`.
        /// </remarks>
        /// <param name="req"></param>
        /// <returns>An Task&lt;ExampleResponse&gt;.</returns>
        [FunctionName(nameof(GetTaskOfIActionResultReturnValue))]
        public Task<IActionResult> GetTaskOfIActionResultReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return Task.FromResult((IActionResult)new OkResult());
        }

        /// <summary>
        /// Function returning an Task&lt;HttpResponseMessage&gt;.
        /// </summary>
        /// <param name="req"></param>
        /// <returns>An HttpResponseMessage.</returns>
        /// <remarks>
        /// Status code will be 200 and return type will be object.
        ///
        /// It is advised to annotate the return type with the `ProducesResponseTypeAttribute`.
        /// </remarks>
        [FunctionName(nameof(GetTaskOfHttpResponseMessageReturnValue))]
        public Task<HttpResponseMessage> GetTaskOfHttpResponseMessageReturnValue(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
