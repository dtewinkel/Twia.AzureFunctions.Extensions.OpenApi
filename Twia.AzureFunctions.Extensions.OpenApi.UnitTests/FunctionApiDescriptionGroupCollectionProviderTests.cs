using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class FunctionApiDescriptionGroupCollectionProviderTests
    {
        private ISwaggerServiceConfigurationStorage _configuration;
        private IHttpFunctionProcessor _functionProcessor;
        private FunctionApiDescriptionGroupCollectionProvider _sut;
        private readonly Assembly _exampleAssembly = AssemblyHelper.GetFunctionAssembly("../../../../Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction", "*.ExampleFunction.dll");

        private readonly string[] _expectedMethodNames =
        {
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.SwaggerFunction.GetSwagger",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.SwaggerFunction.GetOpenApiV3",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleReturnTypes.GetVoidReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleReturnTypes.GetIActionResultReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleReturnTypes.GetHttpResponseMessageReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleReturnTypes.GetExampleResponseReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleReturnTypes.GetExampleResponseArrayReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleAsyncReturnTypes.GetTaskReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleAsyncReturnTypes.GetTaskOfIActionResultReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleAsyncReturnTypes.GetTaskOfHttpResponseMessageReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleAsyncReturnTypes.GetTaskOfExampleResponseReturnValue",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleResponseAttributes.GetVoidProducesResponseType",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleResponseAttributes.GetExampleResponseProducesResponseType",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleResponseAttributes.GetMultipleProducesResponseTypes",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleGrouping.NoGrouping",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleGrouping.GroupV1",
            "Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions.ExampleGrouping.GroupV2"
        };


        [TestInitialize]
        public void TestInitialize()
        {
            _configuration = A.Fake<ISwaggerServiceConfigurationStorage>();
            _functionProcessor = A.Fake<IHttpFunctionProcessor>();

            A.CallTo(() => _configuration.FunctionAssembly).Returns(_exampleAssembly);

            _sut = new FunctionApiDescriptionGroupCollectionProvider(_configuration, _functionProcessor);
        }

        [TestMethod]
        public void Constructor_WithNullForConfiguration_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FunctionApiDescriptionGroupCollectionProvider(null, _functionProcessor);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configuration");
        }

        [TestMethod]
        public void Constructor_WithNullForFunctionProcessor_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FunctionApiDescriptionGroupCollectionProvider(_configuration, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("httpFunctionMethodProcessor");
        }


        [TestMethod]
        public void ApiDescriptionGroups_ReturnsApiDescriptionGroupCollectionWithVersion1()
        {
            var result = _sut.ApiDescriptionGroups;

            result.Version.Should().Be(1);
        }

        [TestMethod]
        public void ApiDescriptionGroups_CallsHttpFunctionProcessorWithCorrectMethods()
        {
            var methods = new List<MethodInfo>();
            A.CallTo(() => _functionProcessor.ProcessHttpFunction(A<MethodInfo>._))
                .Invokes((MethodInfo methodInfo) => methods.Add(methodInfo))
                .Returns(new ApiDescriptionGroup("a", new List<ApiDescription>()));

            var result = _sut.ApiDescriptionGroups;

            result.Should().NotBeNull();
            var actualMethodNames = methods.Select(method => $"{method?.DeclaringType?.FullName ?? "?"}.{method?.Name ?? "?"}");
            actualMethodNames.Should().BeEquivalentTo(_expectedMethodNames);
        }

        [TestMethod]
        public void ApiDescriptionGroups_ReturnsAllApiDescriptionGroups()
        {
            var expectedApiDescriptions = new List<ApiDescriptionGroup>();
            A.CallTo(() => _functionProcessor.ProcessHttpFunction(A<MethodInfo>._))
                .ReturnsLazily(() =>
                {
                    var returnValue = new ApiDescriptionGroup("a", new List<ApiDescription>());
                    expectedApiDescriptions.Add(returnValue);
                    return returnValue;
                });


            var result = _sut.ApiDescriptionGroups;

            result.Should().NotBeNull();
            A.CallTo(() => _functionProcessor.ProcessHttpFunction(A<MethodInfo>._))
                .MustHaveHappened(_expectedMethodNames.Length, Times.Exactly);
            result.Items.Should().BeEquivalentTo(expectedApiDescriptions);
        }

    }
}