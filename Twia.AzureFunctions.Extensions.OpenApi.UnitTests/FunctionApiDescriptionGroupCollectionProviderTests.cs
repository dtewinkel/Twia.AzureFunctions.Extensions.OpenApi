using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Azure.WebJobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class FunctionApiDescriptionGroupCollectionProviderTests
    {
        private IOpenApiServiceConfigurationStorage _configuration;
        private IHttpFunctionProcessor _functionProcessor;
        private FunctionApiDescriptionGroupCollectionProvider _sut;
        private readonly Assembly _exampleAssembly = AssemblyHelper.GetFunctionAssembly("../../../../Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction", "*.ExampleFunction.dll");
        private string[] _expectedMethodNames;

        [TestInitialize]
        public void TestInitialize()
        {
            var types = _exampleAssembly.GetTypes()
                .Where(t => !t.HasAttribute<OpenApiIgnoreAttribute>());
            _expectedMethodNames = types
                .SelectMany(t => t.GetMethods())
                .Where(m => m.HasAttribute<FunctionNameAttribute>())
                .Where(m => m.GetParameters().Any(p => p.HasAttribute<HttpTriggerAttribute>()))
                .Where(m => !m.HasAttribute<OpenApiIgnoreAttribute>())
                .Where(m => !m.GetAttributes<ApiExplorerSettingsAttribute>().Any(a => a.IgnoreApi))
                .Select(methodInfo => $"{methodInfo.DeclaringType?.FullName ?? ""}.{methodInfo.Name}")
                .ToArray();

            _configuration = A.Fake<IOpenApiServiceConfigurationStorage>();
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