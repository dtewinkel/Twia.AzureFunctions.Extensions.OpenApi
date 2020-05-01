using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

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
        public void GetApiParameterDescriptions_WithBodyParameter_ReturnsBodyParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.BodyParameter));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "body",
                Type = typeof(RequestType),
                Source = BindingSource.Body,
            };

            var parameters = _sut.GetApiParameterDescriptions(method, null);

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(1);
            var parameter = parameters.Single();
            parameter.Should().BeEquivalentTo(expectedParameter);
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithPathParameter_ReturnsPathParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.PathParameter));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "param",
                Type = typeof(string),
                Source = BindingSource.Path,
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = false
                },
                ParameterDescriptor = new ControllerParameterDescriptor
                {
                    Name = "param",
                    ParameterType = typeof(string),
                    ParameterInfo = method.GetParameters()[1]
                }
            };

            var parameters = _sut.GetApiParameterDescriptions(method, "/path/{param}");

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(1);
            var parameter = parameters.Single();
            parameter.Should().BeEquivalentTo(expectedParameter);
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithOptionalPathParameter_ReturnsPathParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.PathParameter));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "param",
                Type = typeof(string),
                Source = BindingSource.Path,
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = true
                },
                ParameterDescriptor = new ControllerParameterDescriptor
                {
                    Name = "param",
                    ParameterType = typeof(string),
                    ParameterInfo = method.GetParameters()[1]
                }
            };

            var parameters = _sut.GetApiParameterDescriptions(method, "/path/{param?}");

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(1);
            var parameter = parameters.Single();
            parameter.Should().BeEquivalentTo(expectedParameter);
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithTypeForPathParameter_ReturnsPathParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.PathParameter));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "param",
                Type = typeof(string),
                Source = BindingSource.Path,
                RouteInfo = new ApiParameterRouteInfo
                {
                    IsOptional = true
                },
                ParameterDescriptor = new ControllerParameterDescriptor
                {
                    Name = "param",
                    ParameterType = typeof(string),
                    ParameterInfo = method.GetParameters()[1]
                }
            };

            var parameters = _sut.GetApiParameterDescriptions(method, "/path/{param:int?}");

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(1);
            var parameter = parameters.Single();
            parameter.Should().BeEquivalentTo(expectedParameter);
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithMultiplePathParameters_ReturnsPathParameters()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.PathParameters));
            var expectedParameters = new List<ApiParameterDescription>
            {
                new ApiParameterDescription
                {
                    Name = "param",
                    Type = typeof(string),
                    Source = BindingSource.Path,
                    RouteInfo = new ApiParameterRouteInfo
                    {
                        IsOptional = false
                    },
                    ParameterDescriptor = new ControllerParameterDescriptor
                    {
                        Name = "param",
                        ParameterType = typeof(string),
                        ParameterInfo = method.GetParameters()[1]
                    }

                },
                new ApiParameterDescription
                {
                    Name = "idParam",
                    Type = typeof(int?),
                    Source = BindingSource.Path,
                    RouteInfo = new ApiParameterRouteInfo
                    {
                        IsOptional = true
                    },
                    ParameterDescriptor = new ControllerParameterDescriptor
                    {
                        Name = "idParam",
                        ParameterType = typeof(int?),
                        ParameterInfo = method.GetParameters()[2]
                    }
                }
            };

            var parameters = _sut.GetApiParameterDescriptions(method, "/path/{param}/{idParam:int?}");

            parameters.Should().NotBeNull();
            parameters.Should().HaveCount(2);
            parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [TestMethod]
        public void GetApiParameterDescriptions_WithBodyAttribute_ReturnsBodyParameter()
        {
            var method = GetMethodInfo(nameof(FunctionMethodTestSource.BodyAttribute));
            var expectedParameter = new ApiParameterDescription
            {
                Name = "request",
                Type = typeof(RequestType),
                Source = BindingSource.Body
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

            public void BodyParameter([HttpTrigger(AuthorizationLevel.Anonymous)] RequestType body)
            {
                // Nothing to do here.
            }

            public void PathParameter([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestMessage req, string param)
            {
                // Nothing to do here.
            }

            public void PathParameters([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestMessage req, string param, int? idParam)
            {
                // Nothing to do here.
            }

            public void BodyAttribute(
                [OpenApiBodyType(typeof(RequestType))]
                [HttpTrigger(AuthorizationLevel.Anonymous)]
                HttpRequestMessage request)
            {
                // Nothing to do here.
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }

    }
}