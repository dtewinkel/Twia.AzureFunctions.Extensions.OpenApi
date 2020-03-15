using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for parameters to HTTP functions.
    /// </summary>
    public class ExampleParameters
    {
        /// <summary>
        /// Function implementing just a default HTTP request parameter.
        /// </summary>
        /// <param name="req">Unused request parameter.</param>
        [FunctionName(nameof(HttpRequestParameter))]
        public void HttpRequestParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to return here.
        }

        /// <summary>
        /// Function implementing just a default HTTP request parameter.
        /// </summary>
        /// <param name="req">The request type in the body.</param>
        [FunctionName(nameof(ExampleRequestBodyParameterThroughAttribute))]
        public void ExampleRequestBodyParameterThroughAttribute(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [OpenApiBodyType(typeof(ExampleRequest))]
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to return here.
        }

        /// <summary>
        /// Function implementing an custom type on the trigger as Body parameter.
        /// </summary>
        /// <param name="req">A example body type.</param>
        [FunctionName(nameof(ExampleRequestBodyParameter))]
        public void ExampleRequestBodyParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] ExampleRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We do something useful here :-)
        }

        /// <summary>
        /// Function implementing an custom type on the trigger as Body parameter.
        /// </summary>
        /// <param name="req">No body.</param>
        /// <param name="id">An ID of some sort, as long as it is an integer number.</param>
        [FunctionName(nameof(ExamplePathParameter))]
        public void ExamplePathParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PathParameters/{id}")] HttpRequestMessage req,
            int id)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We do something useful here :-)
        }

        /// <summary>
        /// Function implementing an custom type on the trigger as Body parameter.
        /// </summary>
        /// <param name="req">A example body type.</param>
        /// <param name="id">An ID of some sort, as long as it is an integer number.</param>
        /// <param name="name">Name of the thing to retrieve.</param>
        /// <returns>An IActionResult.</returns>
        [FunctionName(nameof(ExampleMultiplePathParameter))]
        public void ExampleMultiplePathParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PathParameters/{name}/{id}")] HttpRequestMessage req,
            int id,
            string name)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We do something useful here :-)
        }

        /// <summary>
        /// Function implementing an custom type on the trigger as Body parameter.
        /// </summary>
        /// <param name="req">The data in the body that the id is referencing to.</param>
        /// <param name="id">An ID of some sort, as long as it is an integer number.</param>
        /// <returns>An IActionResult.</returns>
        [FunctionName(nameof(ExampleBodyAndPathParameter))]
        public void ExampleBodyAndPathParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PathParameters/{id}")] ExampleRequest req,
            int id)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We do something useful here :-)
        }

        /// <summary>
        /// Function implementing a string as the body parameter.
        /// </summary>
        /// <param name="req">A example string.</param>
        [FunctionName(nameof(ExampleStringParameter))]
        public void ExampleStringParameter(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] string req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We do something useful here :-)
        }

        /// <summary>
        /// Function implementing a cancellation token, which is to be ignored.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        [FunctionName(nameof(IgnoreOtherParameters))]
        public async Task IgnoreOtherParameters(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, ILogger logger, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            logger.LogInformation("Hi!");
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
