using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

#pragma warning disable IDE0060 // Remove unused parameter
// ReSharper disable UnusedParameter.Global

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for grouping.
    /// </summary>
    public class ExampleGrouping
    {
        /// <summary>
        /// HTTP trigger with no specific group defined.
        /// </summary>
        /// <param name="req"></param>
        [FunctionName(nameof(NoGrouping))]
        public void NoGrouping(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// HTTP trigger specific to group "v1".
        /// </summary>
        /// <param name="req"></param>
        [FunctionName(nameof(GroupV1))]
        [ApiExplorerSettings(GroupName = "v1")]
        public void GroupV1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// HTTP trigger specific to group "v2".
        /// </summary>
        /// <param name="req"></param>
        [FunctionName(nameof(GroupV2))]
        [ApiExplorerSettings(GroupName = "v2")]
        public void GroupV2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            // Nothing to do here.
        }
    }
}
