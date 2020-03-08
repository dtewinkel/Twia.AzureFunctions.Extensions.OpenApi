using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionParameterProcessorTests
    {
        private IModelMetadataProvider _modelMetadataProvider;
        private HttpFunctionParameterProcessor _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _modelMetadataProvider = A.Fake<IModelMetadataProvider>();

            _sut = new HttpFunctionParameterProcessor(_modelMetadataProvider);
        }

        [TestMethod]
        public void Constructor_WithNullForModelMetadataProvider_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new HttpFunctionResponseProcessor(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("modelMetadataProvider");
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithNullForFunctionMethod_ThrowsException()
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            Action action = () =>
            {
                 _sut.GetApiParameterDescriptions(null, "");
            };

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("functionMethod");
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithNoParameters_ReturnsEmptySet()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.NoParameters));

            var parameters = _sut.GetApiParameterDescriptions(method, null);

            parameters.Should().NotBeNull();
            parameters.Should().BeEmpty();
        }


        [TestMethod]
        public void GetApiParameterDescriptions_WithBodyParameters_ReturnsBodyParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.BodyParameters));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "body",
                Type = typeof(RequestType),
                Source = BindingSource.Body,
                ParameterDescriptor = new ParameterDescriptor
                {
                    Name = "body",
                    ParameterType = typeof(RequestType)
                }
            };

            var parameters = _sut.GetApiParameterDescriptions(method, null);

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(1);
            var parameter = parameters.Single();
            parameter.Should().BeEquivalentTo(expectedParameter);
        }

        private static MethodInfo GetMethodInfo(string methodName)
        {
            return typeof(HttpFunctionParameterProcessorTests.FunctionMethodTestSource).GetMethod(methodName);
        }

        private class RequestType
        {
        }

        private class FunctionMethodTestSource
        {
#pragma warning disable IDE0060 // Remove unused parameter
            public void NoParameters([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestMessage req)
            {
                // Nothing to do here.
            }

            public void BodyParameters([HttpTrigger(AuthorizationLevel.Anonymous)] RequestType body)
            {
                // Nothing to do here.
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }

    }
}