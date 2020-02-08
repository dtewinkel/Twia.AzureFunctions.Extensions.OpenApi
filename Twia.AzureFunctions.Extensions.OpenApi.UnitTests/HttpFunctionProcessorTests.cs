using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionProcessorTests
    {
        private const string _routePrefix = "aRoute";
        private HttpFunctionResponseProcessor _httpFunctionResponseProcessor;
        private IOptions<HttpOptions> _httpOptions;
        private HttpFunctionProcessor _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpFunctionResponseProcessor = A.Fake<HttpFunctionResponseProcessor>();
            _httpOptions = A.Fake<IOptions<HttpOptions>>();
            A.CallTo(() => _httpOptions.Value).Returns(new HttpOptions() { RoutePrefix = _routePrefix } );

            _sut = new HttpFunctionProcessor(_httpFunctionResponseProcessor, _httpOptions);
        }

        [TestMethod]
        public void Constructor_WithNullForHttpFunctionResponseProcessor_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionProcessor(null, _httpOptions);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionResponseProcessor");
        }

        [TestMethod]
        public void Constructor_WithNullForHttpOptions_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionProcessor(_httpFunctionResponseProcessor, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpOptions");
        }

        [TestMethod]
        public void ProcessHttpFunction_WithNullForHttpFunctionMethod_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => _sut.ProcessHttpFunction(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionMethod");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestSingleMethod_GetsGroupName()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestSingleMethod);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            apiDescriptionGroup.GroupName.Should().Be(methodName);
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestSingleMethod_GetsSingleGroup()
        {
            var expectedMethods = new[] { "GET" };
            var methodName = nameof(FunctionMethodTestSource.MetadataTestSingleMethod);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescriptions = apiDescriptionGroup.Items;
            ValidateApiDescriptions(apiDescriptions, expectedMethods, methodName);
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestMultipleMethods_GetsGroups()
        {
            var expectedMethods = new[] { "GET", "PUT", "DELETE" };
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestMultipleMethods);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescriptions = apiDescriptionGroup.Items;
            ValidateApiDescriptions(apiDescriptions, expectedMethods, methodName);
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestNoMethod_GetsGroups()
        {
            var expectedMethods = new [] { "GET", "POST", "PUT", "DELETE", "HEAD", "PATCH", "OPTIONS" };

            const string methodName = nameof(FunctionMethodTestSource.MetadataTestNoMethod);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescriptions = apiDescriptionGroup.Items;
            ValidateApiDescriptions(apiDescriptions, expectedMethods, methodName);
        }

        private static void ValidateApiDescriptions(IReadOnlyList<ApiDescription> apiDescriptions, IReadOnlyCollection<string> expectedMethods, string methodName)
        {
            apiDescriptions.Should().NotBeNull();
            apiDescriptions.Should().HaveCount(expectedMethods.Count);
            foreach (var expectedMethod in expectedMethods)
            {
                var apiDescription = apiDescriptions.Single(desc => desc.HttpMethod == expectedMethod);
                apiDescription.GroupName.Should().BeNull();
                apiDescription.RelativePath.Should().Be($"{_routePrefix}/{methodName}");
            }
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestApiGroup_SetsApiGroup()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestApiGroup);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.GroupName.Should().Be("MyGroup");
            apiDescription.HttpMethod.Should().Be("GET");
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/{methodName}");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestSimpleRoute_GetsCorrectRoute()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestSimpleRoute);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/MySimpleRoute");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestMetadataTestRouteWithSegments_GetsCorrectRoute()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestRouteWithSegments);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/Route/With/Segments");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestRouteWithPlaceholders_GetsCorrectRoute()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestRouteWithPlaceholders);
            var method = typeof(FunctionMethodTestSource).GetMethod(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/Route/{{name}}/{{date}}");
        }

        private class FunctionMethodTestSource
        {
#pragma warning disable IDE0060 // Remove unused parameter
            [FunctionName(nameof(MetadataTestNoMethod))]
            public void MetadataTestNoMethod([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
            {
                // Nothing to do here.
            }


            [FunctionName(nameof(MetadataTestSingleMethod))]
            public void MetadataTestSingleMethod([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
            {
                // Nothing to do here.
            }

            [FunctionName(nameof(MetadataTestMultipleMethods))]
            public void MetadataTestMultipleMethods([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete")] HttpRequest req)
            {
                // Nothing to do here.
            }

            [FunctionName(nameof(MetadataTestApiGroup))]
            [ApiExplorerSettings(GroupName = "MyGroup")]
            public void MetadataTestApiGroup([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
            {
                // Nothing to do here.
            }

            [FunctionName(nameof(MetadataTestSimpleRoute))]
            public void MetadataTestSimpleRoute([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "MySimpleRoute")] HttpRequest req)
            {
                // Nothing to do here.
            }

            [FunctionName(nameof(MetadataTestRouteWithSegments))]
            public void MetadataTestRouteWithSegments(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Route/With/Segments")] HttpRequest req,
                string name,
                string date)
            {
                // Nothing to do here.
            }

            [FunctionName(nameof(MetadataTestRouteWithPlaceholders))]
            public void MetadataTestRouteWithPlaceholders(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Route/{name}/{date?}/")] HttpRequest req,
                string name,
                string date)
            {
                // Nothing to do here.
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }
    }
}