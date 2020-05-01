using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

[assembly:HeaderParameter("from-assembly", Description = "A header parameter, set at assembly level.", IsRequired = false)]

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Function class showing the use of header parameters.
    /// </summary>
    [HeaderParameter("from-class", Description = "A header parameter at class level.")]
    [HeaderParameter("from-class-optional", Description = "A header parameter at class level.", IsRequired = false)]
    [HeaderParameter("from-class-number", Description = "A integer header parameter at class level.", Type = typeof(int))]
    [HeaderParameter("from-class-complex", Description = "A complex header parameter at class level.", Type = typeof(ExampleRequest))]
    [HeaderParameter("from-class-ignore-method", Description = "An ignored header parameter at assembly level.")]
    public class ExampleHeaderParameters
    {
        /// <summary>
        /// Function showing the use of header parameters.
        /// </summary>
        /// <param name="req">Unused request parameter.</param>
        [HeaderParameter("from-method", Description = "A header parameter at method level.")]
        [HeaderParameter("from-method-optional", Description = "A header parameter at method level.", IsRequired = false)]
        [HeaderParameter("from-method-number", Description = "A integer header parameter at method level.", Type = typeof(int))]
        [HeaderParameter("from-method-complex", Description = "A complex header parameter at method level.", Type = typeof(ExampleRequest))]
        [IgnoreHeaderParameter("from-class-ignore-method")]
        [FunctionName(nameof(HttpInheritedHeaderParameters))]
        public void HttpInheritedHeaderParameters(
            // ReSharper disable once UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Nothing to return here.
        }
    }
}
