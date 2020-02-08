using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction
{
    /// <summary>
    /// Functions to get Swagger or Open API documentation.
    /// </summary>
    public class SwaggerFunction
    {
        private readonly ISwaggerService _swaggerService;

        /// <summary>
        /// Create a new instance of the SwaggerFunction type.
        /// </summary>
        /// <param name="swaggerService">The service implementation to use.</param>
        public SwaggerFunction(ISwaggerService swaggerService)
        {
            _swaggerService = swaggerService;
        }

        /// <summary>
        /// Get the Open API json file.
        /// </summary>
        /// <param name="req">The request on the HTTP trigger.</param>
        /// <param name="documentName">The name of the document version to retrieve. Can be 'v1' or 'v2'. Optional. If not set 'v1' will be used.</param>
        /// <returns>The Swagger documentation for all documented HTTP triggers in this function.</returns>
        /// <response code="200">Json representation of documentation.</response>
        [ProducesResponseType(typeof(string), 200)]
        [FunctionName(nameof(GetOpenApiV3))]
        public HttpResponseMessage GetOpenApiV3(
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi/v3/json/{documentName?}")] HttpRequestMessage req,
            string documentName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var documentJson = _swaggerService.GetSwaggerJson(documentName ?? "v1");
            return new HttpResponseMessage
            {
                Content = new StringContent(documentJson, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Get the Swagger json file.
        /// </summary>
        /// <param name="req">The request on the HTTP trigger.</param>
        /// <param name="documentName">The name of the document version to retrieve. Can be 'v1' or 'v2'. Optional. If not set 'v1' will be used.</param>
        /// <returns>The Swagger documentation for all documented HTTP triggers in this function.</returns>
        ///  <response code="200">Json representation of documentation.</response>
        [ProducesResponseType(typeof(string), 200)]
        [FunctionName(nameof(GetSwagger))]
        public HttpResponseMessage GetSwagger(
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi/v2/json/{documentName?}")] HttpRequestMessage req,
            string documentName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var documentJson = _swaggerService.GetSwaggerJson(documentName ?? "v1", openApiSpecVersion: OpenApiSpecVersion.OpenApi2_0);
            return new HttpResponseMessage
            {
                Content = new StringContent(documentJson, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Get swagger documentation HTML page.
        /// </summary>
        /// <param name="req">The request on the HTTP trigger.</param>
        /// <param name="documentName">The name of the document version to retrieve. Can be 'v1' or 'v2'. Optional. If not set 'v1' will be used.</param>
        /// <returns>The Swagger documentation UI for all documented HTTP triggers in this function.</returns>
        ///  <response code="200">HTTP page with the generated swagger documentation.</response>
        [ProducesResponseType(typeof(string), 200)]
        [FunctionName(nameof(GetSwaggerUi))]
        [OpenApiIgnore]
        public HttpResponseMessage GetSwaggerUi(
#pragma warning disable IDE0060 // Remove unused parameter
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi/ui/{documentName?}")] HttpRequestMessage req,
            string documentName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var html = ReadFromResource("Resources.SwaggerUi.index.html");

            html = html.Replace("{{SwaggerJsonUrl}}", $"/api/openapi/v3/json/{documentName ?? "v1"}");

            return new HttpResponseMessage
            {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
        }

        private static string ReadFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream($"Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.{resourceName}");
            if (stream == null)
            {
                throw new ArgumentException($@"'{resourceName}' seems not to be an existing resource.", nameof(resourceName));
            }
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }
}
