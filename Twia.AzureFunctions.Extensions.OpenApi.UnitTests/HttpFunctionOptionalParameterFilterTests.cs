using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionOptionalParameterFilterTests
    {
        private class TestParameterDescription
        {
            public TestParameterDescription(string name, bool hasRouteInfo, bool isOptional)
            {
                Name = name;
                HasRouteInfo = hasRouteInfo;
                IsOptional = isOptional;
            }

            public string Name { get; }
            public bool HasRouteInfo { get; }
            public bool IsOptional { get; }
        }

        private readonly TestParameterDescription[] _inputApiParameters =
        {
            new TestParameterDescription("stringParam", true, true),
            new TestParameterDescription("integerParam", true, false),
            new TestParameterDescription("booleanParam", false, true)
        };

        private readonly TestParameterDescription[] _expectedApiParameters =
        {
            new TestParameterDescription("stringParam", false, true),
            new TestParameterDescription("integerParam", false, false),
            new TestParameterDescription("booleanParam", false, false)
        };

        private OpenApiOperation _operation;
        private OperationFilterContext _context;
        private ISchemaGenerator _schemaRegistry;
        private SchemaRepository _schemaRepository;
        private HttpFunctionOptionalParameterFilter _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>(_inputApiParameters.Select(
                    param => new OpenApiParameter
                    {
                        Name = param.Name,
                        Required = true,
                    }))
            };
            var method = typeof(FunctionMethodTestSource).GetMethod(nameof(FunctionMethodTestSource.WithParameters));
            _schemaRegistry = A.Fake<ISchemaGenerator>();
            _schemaRepository = new SchemaRepository();
            var apiDescription = new ApiDescription();
            foreach (var inputApiParameter in _inputApiParameters)
            {
                var parameterDescription = new ApiParameterDescription
                {
                    Name = inputApiParameter.Name,
                    RouteInfo = inputApiParameter.HasRouteInfo ? new ApiParameterRouteInfo { IsOptional =  inputApiParameter.IsOptional } : null
                };
                apiDescription.ParameterDescriptions.Add(parameterDescription);
            }

            _context = new OperationFilterContext(apiDescription, _schemaRegistry, _schemaRepository, method);
            _sut = new HttpFunctionOptionalParameterFilter();
        }

        [TestMethod]
        public void Apply_WithNullForOperation_ThrowsException()
        {
            Action action = () => _sut.Apply(null, _context);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("operation");
        }

        [TestMethod]
        public void Apply_WithNullForContext_ThrowsException()
        {
            Action action = () => _sut.Apply(_operation, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void Apply_ForStringParameter_ReturnsCorrectResult()
        {
            _sut.Apply(_operation, _context);

            ValidateResult(_expectedApiParameters);
        }

        private void ValidateResult(TestParameterDescription[] expectedParameters)
        {
            _operation.Parameters.Should().HaveCount(expectedParameters.Length);
            foreach (var expectedParameter in expectedParameters)
            {
                var openApiParameter = _operation.Parameters.SingleOrDefault(param => param.Name == expectedParameter.Name);
                openApiParameter
                    .Should()
                    .NotBeNull($"an OpenAPiParameter with name {expectedParameter.Name} was not found matching all the requirements");
                openApiParameter.Required.Should().Be(!expectedParameter.IsOptional);
            }
        }

        private static class FunctionMethodTestSource
        {
            public static void WithParameters(string stringParam, int? integerParam, bool booleanParam)
            {
                // Just for testing.
            }
        }
    }
}