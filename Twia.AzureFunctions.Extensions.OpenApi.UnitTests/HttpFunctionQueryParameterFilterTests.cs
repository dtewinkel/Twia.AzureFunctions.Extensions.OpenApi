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

[assembly: QueryParameter("assembly-default")]
[assembly: QueryParameter("assembly-complete", Description = "Description of assembly-complete!", IsRequired = false, Type = typeof(decimal))]
[assembly: QueryParameter("assembly-to-ignore1")]
[assembly: QueryParameter("assembly-to-ignore2")]

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class HttpFunctionQueryParameterFilterTests
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
            NewOpenApiParameter("source1-default-QueryParameters"),
            NewOpenApiParameter("source1-complete-QueryParameters", true, true, typeof(int)),
            NewOpenApiParameter("source1-default-NoIgnoreQueryParameters")
        };

        private static OpenApiParameter NewOpenApiParameter(string name, bool generateDescription = false, bool setIsRequiredToFalse = false, Type setType = null)
        {
            var openApiParameter = new OpenApiParameter
            {
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
            var sut = new HttpFunctionQueryParameterFilter();
            var context = A.Fake<OperationFilterContext>();

            Action action = () => sut.Apply(null, context);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("operation");
        }

        [TestMethod]
        public void Apply_WithNullForContext_ThrowsException()
        {
            var sut = new HttpFunctionQueryParameterFilter();

            Action action = () => sut.Apply(_operation, null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void Apply_ForNoOwnQueryParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.NoOwnQueryParameters));
            var expectedResultIds = _functionMethodTestSource1.NoOwnQueryParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionQueryParameterFilter();

            sut.Apply(_operation, context);

            ValidateResult(expectedResultIds);
        }

        [TestMethod]
        public void Apply_ForQueryParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.QueryParameters));
            var expectedResultIds = _functionMethodTestSource1.QueryParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionQueryParameterFilter();

            sut.Apply(_operation, context);

            ValidateResult(expectedResultIds);
        }

        [TestMethod]
        public void Apply_ForNoIgnoreQueryParameters_ReturnsCorrectResult()
        {
            var method = GetMethodInfo1(nameof(FunctionMethodTestSource1.NoIgnoreQueryParameters));
            var expectedResultIds = _functionMethodTestSource1.NoIgnoreQueryParameters();
            var context = CreateOperationFilterContext(method);

            var sut = new HttpFunctionQueryParameterFilter();

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
            return typeof(HttpFunctionQueryParameterFilterTests.FunctionMethodTestSource1).GetMethod(methodName);
        }

        [QueryParameter("source1-default")]
        [QueryParameter("source1-complete", Description = "Description of source1-complete!", IsRequired = false, Type = typeof(short))]
        [QueryParameter("source1-to-ignore")]
        [IgnoreQueryParameter("assembly-to-ignore1")]
        private class FunctionMethodTestSource1
        {
            [IgnoreQueryParameter("source1-to-ignore")]
            public int[] NoOwnQueryParameters()
            {
                return new[] {0, 1, 3, 4, 5};
            }

            [QueryParameter("source1-default-QueryParameters")]
            [QueryParameter("source1-complete-QueryParameters", Description = "Description of source1-complete-QueryParameters!", IsRequired = false, Type = typeof(int))]
            [IgnoreQueryParameter("source1-to-ignore")]
            [IgnoreQueryParameter("assembly-to-ignore2")]
            public int[] QueryParameters()
            {
                return new[] { 0, 1, 4, 5, 7, 8 };
            }

            [QueryParameter("source1-default-NoIgnoreQueryParameters")]
            public int[] NoIgnoreQueryParameters()
            {
                return new[] { 0, 1, 3, 4, 5, 6, 9 };
            }
        }
    }
}