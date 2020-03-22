using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;
using Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions;

[assembly:QueryParameter("from-assembly", Description = "A query parameter at assembly level")]
[assembly:QueryParameter("from-assembly-optional", Description = "A query parameter at assembly level", IsRequired = false)]
[assembly:QueryParameter("from-assembly-number", Description = "A integer query parameter at assembly level", Type = typeof(int))]
[assembly:QueryParameter("from-assembly-complex", Description = "A complex query parameter at assembly level", Type = typeof(ExampleRequest))]
[assembly: QueryParameter("from-assembly-ignore-class", Description = "An ignored query parameter at assembly level")]
[assembly: QueryParameter("from-assembly-ignore-method", Description = "An ignored query parameter at assembly level")]

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Provides examples for parameters to HTTP functions.
    /// </summary>
    [QueryParameter("from-class", Description = "A query parameter at class level")]
    [QueryParameter("from-class-optional", Description = "A query parameter at class level", IsRequired = false)]
    [QueryParameter("from-class-number", Description = "A integer query parameter at class level", Type = typeof(int))]
    [QueryParameter("from-class-complex", Description = "A complex query parameter at class level", Type = typeof(ExampleRequest))]
    [QueryParameter("from-class-ignore-method", Description = "An ignored query parameter at assembly level")]
    [IgnoreQueryParameter("from-assembly-ignore-class")]
    public class ExampleQueryParameters
    {
        /// <summary>
        /// Function implementing just a default HTTP request parameter.
        /// </summary>
        /// <param name="req">Unused request parameter.</param>
        [QueryParameter("from-method", Description = "A query parameter at method level")]
        [QueryParameter("from-method-optional", Description = "A query parameter at method level", IsRequired = false)]
        [QueryParameter("from-method-number", Description = "A integer query parameter at method level", Type = typeof(int))]
        [QueryParameter("from-method-complex", Description = "A complex query parameter at method level", Type = typeof(ExampleRequest))]
        [IgnoreQueryParameter("from-assembly-ignore-method")]
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
