using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestAssembly.Client;

namespace TestAssembly.Function
{
    /// <summary>
    /// Functions for unit tests and integration testing.
    /// </summary>
    public static class DemoFunction
    {
        /// <summary>
        /// Test HTTP triggered function.
        /// </summary>
        /// <param name="req">The incoming request.</param>
        /// <param name="log">The logger.</param>
        /// <param name="name">The name of the caller.</param>
        /// <returns>A TestObject with a message.</returns>
        ///  <response code="200">A TestObject with a greeting.</response>
        ///  <response code="400">A TestObject with an error message.</response>
        [ProducesResponseType(typeof(TestObject), 200)]
        [ProducesResponseType(typeof(TestObject), 400)]
        [FunctionName(nameof(Test))]
        public static async Task<IActionResult> Test(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "test/{name?}")] HttpRequest req,
            ILogger log,
            string name)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            name ??= req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            var testObject = new TestObject();
            if (name != null)
            {
                testObject.Message = $"Hello, {name}";
                return new OkObjectResult(testObject);
            }

            testObject.Message = "Oops, no name given!";
            return new ObjectResult(testObject)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
