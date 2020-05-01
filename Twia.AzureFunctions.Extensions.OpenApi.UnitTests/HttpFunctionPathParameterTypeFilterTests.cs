using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionPathParameterTypeFilterTests
    {
        private class TestParameterDescription
        {
            public TestParameterDescription(string name, Type type, ParameterLocation location = ParameterLocation.Path)
            {
                Name = name;
                Type = type;
                Location = location;
            }

            public string Name { get; }
            public Type Type { get; }
            public ParameterLocation Location { get; }
        }

        private readonly TestParameterDescription[] _inputApiParameters =
        {
            new TestParameterDescription("stringParam", typeof(string)),
            new TestParameterDescription("integerParam", typeof(int)),
            new TestParameterDescription("queryParam", typeof(string), ParameterLocation.Query),
            new TestParameterDescription("booleanParam", typeof(bool)),
            new TestParameterDescription("headerParam", typeof(int?), ParameterLocation.Header)
        };

        private readonly TestParameterDescription[] _expectedApiParameters =
        {
            new TestParameterDescription("stringParam", typeof(string)),
            new TestParameterDescription("integerParam", typeof(int)),
            new TestParameterDescription("booleanParam", typeof(bool)),
            new TestParameterDescription("queryParam", null, ParameterLocation.Query),
            new TestParameterDescription("headerParam", null, ParameterLocation.Header)
        };

        private OpenApiOperation _operation;
        private OperationFilterContext _context;
        private ISchemaGenerator _schemaRegistry;
        private SchemaRepository _schemaRepository;
        private HttpFunctionPathParameterTypeFilter _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>(_inputApiParameters.Select(
                    param => new OpenApiParameter
                    {
                        Name = param.Name,
                        In = param.Location
                    }))
            };
            var method = typeof(FunctionMethodTestSource).GetMethod(nameof(FunctionMethodTestSource.WithParameters));
            _schemaRegistry = A.Fake<ISchemaGenerator>();
            _schemaRepository = new SchemaRepository();
            var apiDescription = new ApiDescription();
            foreach (var inputApiParameter in _inputApiParameters.Where(param => param.Location == ParameterLocation.Path))
            {
                var parameterDescription = new ApiParameterDescription
                {
                    Name = inputApiParameter.Name,
                    Type = inputApiParameter.Type
                };
                apiDescription.ParameterDescriptions.Add(parameterDescription);
            }

            _context = new OperationFilterContext(apiDescription, _schemaRegistry, _schemaRepository, method);
            A.CallTo(() => _schemaRegistry.GenerateSchema(A<Type>._, _schemaRepository, A<MemberInfo>._, A<ParameterInfo>._))
                .ReturnsLazily((Type type, SchemaRepository repository, MemberInfo memberInfo, ParameterInfo parameterInfo) =>
                    new OpenApiSchema
                    {
                        Type = type.Name
                    });
            _sut = new HttpFunctionPathParameterTypeFilter();
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
                var openApiParameter = _operation.Parameters.SingleOrDefault(param =>
                    param.Name == expectedParameter.Name
                    && param.In == expectedParameter.Location
                    );
                openApiParameter.Should()
                    .NotBeNull($"an OpenAPiParameter with name {expectedParameter.Name} was not found matching all the requirements");
                if (expectedParameter.Location == ParameterLocation.Path)
                {
                    openApiParameter.Schema.Should().NotBeNull("Schema should be set for non-Path parameters.");
                    openApiParameter.Schema.Type.Should().Be(expectedParameter.Type.Name);
                }
                else
                {
                    openApiParameter.Schema.Should().BeNull("Schema should not be set for non-Path parameters.");
                }
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