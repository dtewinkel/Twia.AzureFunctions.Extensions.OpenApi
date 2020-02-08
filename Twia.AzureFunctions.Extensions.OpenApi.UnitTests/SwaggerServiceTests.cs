using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class SwaggerServiceTests
    {
        private ISwaggerProvider _swaggerProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _swaggerProvider = A.Fake<ISwaggerProvider>();
        }

        [TestMethod]
        public void Constructor_WithNullForSwaggerProvider_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new SwaggerService(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("swaggerProvider");
        }

        [TestMethod]
        public void GetSwaggerJson_WithNullForDocumentName_ThrowsException()
        {
            var sut = new SwaggerService(_swaggerProvider);

            Action action = () => sut.GetSwaggerJson(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("documentName");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        [DataRow("   ")]
        public void GetSwaggerJson_WithInvalidDataForDocumentName_ThrowsException(string documentName)
        {
            var sut = new SwaggerService(_swaggerProvider);

            Action action = () => sut.GetSwaggerJson(documentName);

            action.Should().Throw<ArgumentException>().And.ParamName.Should().Be("documentName");
        }

        [TestMethod]
        public void GetSwaggerJson_PassesOnDocumentName()
        {
            var documentName = "myDocumentName";
            var openApiDocument = new OpenApiDocument();

            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).Returns(openApiDocument);

            var sut = new SwaggerService(_swaggerProvider);

            var jsonDocument = sut.GetSwaggerJson(documentName);

            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).MustHaveHappenedOnceExactly();
            jsonDocument.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void GetSwaggerJson_ForOpenApi2_ReturnsCorrectJson()
        {
            const string documentName = "myDocumentName";
            var expectedJson = ReadJsonFromResource("Testdata.SwaggerService.OpenApi2_0.json");
            var openApiDocument = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Description = "Test document",
                    Version = "v1",
                    Title = "Test"
                }
            };
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).Returns(openApiDocument);

            var sut = new SwaggerService(_swaggerProvider);

            var jsonDocument = sut.GetSwaggerJson(documentName, openApiSpecVersion: OpenApiSpecVersion.OpenApi2_0);

            var json = JToken.Parse(jsonDocument);

            json.Should().BeEquivalentTo(expectedJson);
        }

        [TestMethod]
        public void GetSwaggerJson_ForOpenApi3_ReturnsCorrectJson()
        {
            const string documentName = "myDocumentName";
            var expectedJson = ReadJsonFromResource("Testdata.SwaggerService.OpenApi3_0.json");
            var openApiDocument = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Description = "Test document 2",
                    Version = "v2",
                    Title = "Test 2"
                }
            };
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).Returns(openApiDocument);

            var sut = new SwaggerService(_swaggerProvider);

            var jsonDocument = sut.GetSwaggerJson(documentName);

            var json = JToken.Parse(jsonDocument);

            json.Should().BeEquivalentTo(expectedJson);
        }


        [TestMethod]
        [DataRow("https://example.com/", "BasePath", "https://example.com", "/BasePath")]
        [DataRow("example.com", "/BasePath", "https://example.com", "/BasePath")]
        [DataRow("example.com/", "BasePath/", "https://example.com", "/BasePath")]
        [DataRow("https://example.com", "/BasePath/", "https://example.com", "/BasePath")]
        [DataRow("https://example.com:443/", "/BasePath/", "https://example.com:443", "/BasePath")]
        [DataRow(null, null, null, null)]
        [DataRow("https://example.com", null, "https://example.com", null)]
        [DataRow("https://example.com", "", "https://example.com", null)]
        [DataRow("https://example.com", "   ", "https://example.com", null)]
        [DataRow("https://example.com", "  / ", "https://example.com", "/")]
        [DataRow("https://example.com", "/example/service/", "https://example.com", "/example/service")]
        [DataRow(null, "/example", null, "/example")]
        [DataRow("  ", "/example", null, "/example")]
        [DataRow("", "/example", null, "/example")]
        public void GetSwaggerJson_ForHostAndBasePath_PassesOnCorrectHostAndBasePath(string host, string basePath, string expectedHost, string expectedBasePath)
        {
            const string documentName = "myDocumentName";
            var openApiDocument = new OpenApiDocument();
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, A<string>._, A<string>._)).Returns(openApiDocument);

            var sut = new SwaggerService(_swaggerProvider);

            var jsonDocument = sut.GetSwaggerJson(documentName,host, basePath);

            jsonDocument.Should().NotBeNullOrWhiteSpace();
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, expectedHost, expectedBasePath)).MustHaveHappenedOnceExactly();
        }

        private static JToken ReadJsonFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream($"Twia.AzureFunctions.Extensions.OpenApi.UnitTests.{resourceName}");
            if (stream == null)
            {
                throw new ArgumentException($@"'{resourceName}' seems not to be an existing resource.", nameof(resourceName));
            }
            using var reader = new StreamReader(stream);
            return JToken.Parse(reader.ReadToEnd());
        }
    }
}