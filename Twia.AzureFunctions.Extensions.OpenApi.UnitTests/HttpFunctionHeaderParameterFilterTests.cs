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
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

[assembly: HeaderParameter("assembly-default")]
[assembly: HeaderParameter("assembly-complete", Description = "Description of assembly-complete!", IsRequired = false, Type = typeof(decimal))]
[assembly: HeaderParameter("assembly-to-ignore1")]
[assembly: HeaderParameter("assembly-to-ignore2")]

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionHeaderParameterFilterTests
    {
        private readonly OpenApiParameter[] _expectedApiParameters = new[]
        {
            NewOpenApiParameter("assembly-default"),
            NewOpenApiParameter("assembly-complete", true, true, typeof(decimal)),
            NewOpenApiParameter("assembly-to-ignore1"),
            NewOpenApiParameter("assembly-to-ignore2"),
            NewOpenApiParameter("source1-default"),
            NewOpenApiParameter("source1-complete", true, true, typeof(short)),
            NewOpenApiParameter("source1-to-ignore"),
            NewOpenApiParameter("source1-default-HeaderParameters"),
            NewOpenApiParameter("source1-complete-HeaderParameters", true, true, typeof(int)),
            NewOpenApiParameter("source1-default-NoIgnoreHeaderParameters")
        };

        private static OpenApiParameter NewOpenApiParameter(string name, bool generateDescription = false, bool setIsRequiredToFalse = false, Type setType = null)
        {
            var openApiParameter = new OpenApiParameter
            {
                In = ParameterLocation.Header,
                Name = name,
                Schema = new OpenApiSchema
                {
                    Type = setType?.Name ?? typeof(string).Name
                },
                Required = !setIsRequiredToFalse
            };
            if (generateDescription)
            {
                openApiParameter.Description = $"Description of {name}!";
            }
            if (setIsRequiredToFalse)
            {
                openApiParameter.Required = false;
            }
            return openApiParameter;
        }

        private OpenApiOperation _operation;
        private FunctionMethodTestSource1 _functionMethodTestSource1;
        private ISchemaGenerator _schemaRegistry;
        private SchemaRepository _schemaRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>()
            };
            _functionMethodTestSource1 = new FunctionMethodTestSource1();
            _schemaRegistry = A.Fake<ISchemaGenerator>();
            _schemaRepository = new SchemaRepository();
        }

        [TestMethod]
        public void Apply_WithNullForOperation_ThrowsException()
        {
            var sut = new HttpFunctionHeaderParameterFilter();
            var context = A.Fake<OperationFilterContext>();

            Action action = () => sut.Apply(null, context);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("operation");
        }

        [TestMethod]
        public void Apply_WithNullForContext_ThrowsException()
        {
            var sut = new HttpFunctionHeaderParameterFilter();

            Action action = () => sut.Apply(_operation, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void Apply_NoOwnHeaderParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.NoOwnHeaderParameters));
            var expectedResultIds = _functionMethodTestSource1.NoOwnHeaderParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionHeaderParameterFilter();

            sut.Apply(_operation, context);

            ValidateResult(expectedResultIds);
        }

        [TestMethod]
        public void Apply_ForHeaderParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.HeaderParameters));
            var expectedResultIds = _functionMethodTestSource1.HeaderParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionHeaderParameterFilter();

            sut.Apply(_operation, context);

            ValidateResult(expectedResultIds);
        }

        [TestMethod]
        public void Apply_ForNoIgnoreHeaderParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.NoIgnoreHeaderParameters));
            var expectedResultIds = _functionMethodTestSource1.NoIgnoreHeaderParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionHeaderParameterFilter();

            sut.Apply(_operation, context);

            ValidateResult(expectedResultIds);
        }

        private OperationFilterContext CreateOperationFilterContext(MethodInfo method)
        {
            A.CallTo(() => _schemaRegistry.GenerateSchema(A<Type>._, _schemaRepository, method, null))
                .ReturnsLazily((Type type, SchemaRepository repository, MemberInfo memberInfo, ParameterInfo parameterInfo) =>
                    new OpenApiSchema
                    {
                        Type = type.Name
                    });
            return new OperationFilterContext(new ApiDescription(), _schemaRegistry, _schemaRepository, method);
        }

        private void ValidateResult(int[] expectedResultIds)
        {
            _operation.Parameters.Should().HaveCount(expectedResultIds.Length);
            foreach (var expectedResultId in expectedResultIds)
            {
                var expected = _expectedApiParameters[expectedResultId];

                var openApiParameter = _operation.Parameters.SingleOrDefault(param =>
                    param.Name == expected.Name
                    && param.Required == expected.Required
                    && param.Schema.Type == expected.Schema.Type
                    && param.Description == expected.Description
                    );
                openApiParameter.Should()
                    .NotBeNull($"an OpenAPiParameter with name {expected.Name} was not found matching all the requirements");
            }
        }

        private static MethodInfo GetMethodInfo1(string methodName)
        {
            return typeof(FunctionMethodTestSource1).GetMethod(methodName);
        }

        [HeaderParameter("source1-default")]
        [HeaderParameter("source1-complete", Description = "Description of source1-complete!", IsRequired = false, Type = typeof(short))]
        [HeaderParameter("source1-to-ignore")]
        [IgnoreHeaderParameter("assembly-to-ignore1")]
        private class FunctionMethodTestSource1
        {
            [IgnoreHeaderParameter("source1-to-ignore")]
            public int[] NoOwnHeaderParameters()
            {
                return new[] {0, 1, 3, 4, 5};
            }

            [HeaderParameter("source1-default-HeaderParameters")]
            [HeaderParameter("source1-complete-HeaderParameters", Description = "Description of source1-complete-HeaderParameters!", IsRequired = false, Type = typeof(int))]
            [IgnoreHeaderParameter("source1-to-ignore")]
            [IgnoreHeaderParameter("assembly-to-ignore2")]
            public int[] HeaderParameters()
            {
                return new[] { 0, 1, 4, 5, 7, 8 };
            }

            [HeaderParameter("source1-default-NoIgnoreHeaderParameters")]
            public int[] NoIgnoreHeaderParameters()
            {
                return new[] { 0, 1, 3, 4, 5, 6, 9 };
            }
        }
    }
}