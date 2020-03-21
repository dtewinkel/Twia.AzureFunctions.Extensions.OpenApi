using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
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
        private IHttpFunctionResponseProcessor _httpFunctionResponseProcessor;
        private IHttpFunctionParameterProcessor _httpFunctionParameterProcessor;
        private IOptions<HttpOptions> _httpOptions;
        private HttpFunctionProcessor _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpFunctionParameterProcessor = A.Fake<IHttpFunctionParameterProcessor>();
            _httpFunctionResponseProcessor = A.Fake<IHttpFunctionResponseProcessor>();
            _httpOptions = A.Fake<IOptions<HttpOptions>>();
            A.CallTo(() => _httpOptions.Value).Returns(new HttpOptions { RoutePrefix = _routePrefix } );
            A.CallTo(() => _httpFunctionParameterProcessor.GetApiParameterDescriptions(A<MethodInfo>._, A<string>._)).Returns(new List<ApiParameterDescription>(0));

            _sut = new HttpFunctionProcessor(_httpFunctionParameterProcessor, _httpFunctionResponseProcessor, _httpOptions);
        }

        [TestMethod]
        public void Constructor_WithNullForHttpFunctionParameterProcessor_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionProcessor(null, _httpFunctionResponseProcessor, _httpOptions);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionParameterProcessor");
        }

        [TestMethod]
        public void Constructor_WithNullForHttpFunctionResponseProcessor_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionProcessor(_httpFunctionParameterProcessor, null, _httpOptions);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionResponseProcessor");
        }

        [TestMethod]
        public void Constructor_WithNullForHttpOptions_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionProcessor(_httpFunctionParameterProcessor, _httpFunctionResponseProcessor, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpOptions");
        }

        [TestMethod]
        public void ProcessHttpFunction_WithNullForHttpFunctionMethod_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => _sut.ProcessHttpFunction(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunction");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestSingleMethod_GetsGroupName()
        {
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestSingleMethod);
            var method = GetMethodInfo(methodName);

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            apiDescriptionGroup.GroupName.Should().Be(methodName);
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestSingleMethod_GetsSingleGroup()
        {
            var expectedMethods = new[] { "GET" };
            const string methodName = nameof(FunctionMethodTestSource.MetadataTestSingleMethod);
            var method = GetMethodInfo(methodName);

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
            var method = GetMethodInfo(methodName);

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
            var method = GetMethodInfo(methodName);

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
            var method = GetMethodInfo(methodName);

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
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestSimpleRoute));

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/MySimpleRoute");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestMetadataTestRouteWithSegments_GetsCorrectRoute()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestRouteWithSegments));

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/Route/With/Segments");
        }

        [TestMethod]
        public void ProcessHttpFunction_ForMetadataTestRouteWithPlaceholders_GetsCorrectRoute()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestRouteWithPlaceholders));

            // ReSharper disable once ObjectCreationAsStatement
            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.RelativePath.Should().Be($"{_routePrefix}/Route/{{name}}/{{date}}/{{id}}/{{anotherId}}");
        }

        [TestMethod]
        public void ProcessHttpFunction_WithRouteSet_PassesMethodAndRouteToHttpFunctionParameterProcessor()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestRouteWithPlaceholders));
            var expectedRoute = "Route/{name}/{date?}/{id:int}/{anotherId:int?}/";

            _sut.ProcessHttpFunction(method);

            A.CallTo(() => _httpFunctionParameterProcessor.GetApiParameterDescriptions(method, expectedRoute))
                .MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void ProcessHttpFunction_AddsApiParameterDescriptors()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestRouteWithPlaceholders));
            var expectedParameterDescriptions = new List<ApiParameterDescription>
            {
                new ApiParameterDescription
                {
                    Name = "1",
                    ParameterDescriptor = new ParameterDescriptor { Name = "1" }
                },
                new ApiParameterDescription
                {
                    Name = "2",
                    ParameterDescriptor = new ParameterDescriptor { Name = "2" }
                },
                new ApiParameterDescription
                {
                    Name = "3",
                    ParameterDescriptor = new ParameterDescriptor { Name = "3" }
                }
            };


            A.CallTo(() => _httpFunctionParameterProcessor.GetApiParameterDescriptions(method, A<string>._))
                .Returns(expectedParameterDescriptions);

            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.ParameterDescriptions.Should().BeEquivalentTo(expectedParameterDescriptions);
            apiDescription.ActionDescriptor.Parameters.Should()
                .BeEquivalentTo(expectedParameterDescriptions.Select(desc => desc.ParameterDescriptor));
        }


        [TestMethod]
        public void ProcessHttpFunction_AddsSupportedResponseTypes()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.MetadataTestRouteWithPlaceholders));
            var expectedResponseTypes = new List<ApiResponseType>
            {
                new ApiResponseType
                {
                    Type = typeof(string)
                },
                new ApiResponseType
                {
                    Type = typeof(void)
                }
            };


            A.CallTo(() => _httpFunctionResponseProcessor.GetResponseTypes(method))
                .Returns(expectedResponseTypes);

            var apiDescriptionGroup = _sut.ProcessHttpFunction(method);

            var apiDescription = apiDescriptionGroup.Items.Single();
            apiDescription.SupportedResponseTypes.Should().BeEquivalentTo(expectedResponseTypes);
        }

        private static MethodInfo GetMethodInfo(string methodName)
        {
            return typeof(FunctionMethodTestSource).GetMethod(methodName);
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
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Route/{name}/{date?}/{id:int}/{anotherId:int?}/")] HttpRequest req,
                string name,
                string date)
            {
                // Nothing to do here.
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }
    }
}