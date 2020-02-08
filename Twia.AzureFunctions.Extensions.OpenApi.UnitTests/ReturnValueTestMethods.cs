using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    /// <summary>
    /// This class contains methods that can be process by various units in unit tests.
    /// </summary>
    public class ReturnValueTestMethods
    {
        [ProducesResponseType(typeof(ResponseType), 200)]
        public IActionResult AnnotatedSingleAttribute()
        {
            return new OkObjectResult(new ResponseType());
        }

        [ProducesResponseType(typeof(ResponseType), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult AnnotatedMultipleAttributes()
        {
            return new OkObjectResult(new ResponseType());
        }

        public IActionResult PlainIActionResult()
        {
            return new OkResult();
        }

        public ResponseType PlainResponseType()
        {
            return new ResponseType();
        }

        public IEnumerable<ResponseType> PlainResponseTypeCollection()
        {
            return new List<ResponseType> {new ResponseType()};
        }

        public HttpResponseMessage PlainHttpResponseMessage()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public void PlainVoid()
        {
            // Nothing to do here.
        }

        public Task<IActionResult> PlainTaskOfIActionResult()
        {
            return Task.FromResult((IActionResult)new OkResult());
        }

        public Task<ResponseType> PlainTaskOfResponseType()
        {
            return Task.FromResult(new ResponseType());
        }

        public Task<HttpResponseMessage> PlainTaskOfHttpResponseMessage()
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        public Task PlainTaskResult()
        {
            return Task.CompletedTask;
        }
    }

    public class ResponseType
    {
        public string Message { get; set; }
    }
}