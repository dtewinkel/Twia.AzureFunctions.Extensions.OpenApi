using Microsoft.Azure.WebJobs;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Non-HTTP trigger functions.
    /// </summary>
    /// <remarks>
    /// As these functions are not a HTTP trigger function, they will not appear in the Open API documentation.
    /// </remarks>
    public static class NonHttpFunctions
    {
        /// <summary>
        /// Function that runs on a timer once every day at midnight.
        /// </summary>
        /// <param name="myTimer">The timer trigger.</param>
        [FunctionName(nameof(Timer))]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Timer([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // We could do something useful here :-).
        }
    }
}
