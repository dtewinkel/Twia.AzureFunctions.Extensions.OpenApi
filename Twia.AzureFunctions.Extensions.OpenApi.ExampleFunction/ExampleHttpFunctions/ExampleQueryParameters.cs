using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

[assembly:QueryParameter("from-assembly", Description = "A query parameter, set at assembly level.", IsRequired = false)]

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Function class showing the use of query parameters.
    /// </summary>
    [QueryParameter("from-class", Description = "A query parameter at class level")]
    [QueryParameter("from-class-optional", Description = "A query parameter at class level", IsRequired = false)]
    [QueryParameter("from-class-number", Description = "A integer query parameter at class level", Type = typeof(int))]
    [QueryParameter("from-class-complex", Description = "A complex query parameter at class level", Type = typeof(ExampleRequest))]
    [QueryParameter("from-class-ignore-method", Description = "An ignored query parameter at assembly level")]
    public class ExampleQueryParameters
    {
        /// <summary>
        /// Function showing the use of query parameters.
        /// </summary>
        /// <param name="req">Unused request parameter.</param>
        [QueryParameter("from-method", Description = "A query parameter at method level")]
        [QueryParameter("from-method-optional", Description = "A query parameter at method level", IsRequired = false)]
        [QueryParameter("from-method-number", Description = "A integer query parameter at method level", Type = typeof(int))]
        [QueryParameter("from-method-complex", Description = "A complex query parameter at method level", Type = typeof(ExampleRequest))]
        [IgnoreQueryParameter("from-class-ignore-method")]
        [FunctionName(nameof(HttpInheritedQueryParameters))]
        public void HttpInheritedQueryParameters(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to return here.
        }
    }
}
