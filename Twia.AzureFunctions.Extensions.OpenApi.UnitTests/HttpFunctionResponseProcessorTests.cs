using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionResponseProcessorTests
    {
        private IModelMetadataProvider _modelMetadataProvider;
        private HttpFunctionResponseProcessor _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _modelMetadataProvider = A.Fake<IModelMetadataProvider>();

            _sut = new HttpFunctionResponseProcessor(_modelMetadataProvider);
        }

        [TestMethod]
        public void Constructor_WithNullForModelMetadataProvider_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionResponseProcessor(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("modelMetadataProvider");
        }

        [TestMethod]
        public void GetResponseTypes_WithNullForHttpFunctionMethod_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => _sut.GetResponseTypes(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionMethod");
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainIActionResult_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainIActionResult));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(object));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainHttpResponseMessage_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainHttpResponseMessage));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(object));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainResponseType_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainResponseType));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(ResponseType));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainResponseTypeCollection_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainResponseTypeCollection));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(IEnumerable<ResponseType>));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForVoid_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainVoid));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(204);
            apiResponseType.Type.Should().Be(typeof(void));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainTaskOfIActionResult_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainTaskOfIActionResult));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(object));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainTaskOfHttpResponseMessage_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainTaskOfHttpResponseMessage));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(object));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForPlainTaskOfResponseType_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainTaskOfResponseType));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(ResponseType));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForTask_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.PlainTaskResult));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(204);
            apiResponseType.Type.Should().Be(typeof(void));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForAnnotatedSingle_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.AnnotatedSingleAttribute));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(1);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(ResponseType));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }

        [TestMethod]
        public void GetResponseTypes_ForAnnotatedMultiple_ReturnsCorrectInfo()
        {
            var method = typeof(ReturnValueTestMethods).GetMethod(nameof(ReturnValueTestMethods.AnnotatedMultipleAttributes));

            // ReSharper disable once ObjectCreationAsStatement
            var result = _sut.GetResponseTypes(method);

            result.Count.Should().Be(2);
            var apiResponseType = result[0];
            apiResponseType.StatusCode.Should().Be(200);
            apiResponseType.Type.Should().Be(typeof(ResponseType));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
            apiResponseType = result[1];
            apiResponseType.StatusCode.Should().Be(204);
            apiResponseType.Type.Should().Be(typeof(void));
            apiResponseType.IsDefaultResponse.Should().BeFalse();
            apiResponseType.ModelMetadata.Should().BeNull();
        }
        /// <summary>
        /// This class contains methods that can be process by various units in unit tests.
        /// </summary>
        private class ReturnValueTestMethods
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
                return new List<ResponseType> { new ResponseType() };
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

        private class ResponseType
        {
        }
    }
}