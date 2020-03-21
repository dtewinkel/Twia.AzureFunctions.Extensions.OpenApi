using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class OpenApiServiceTests
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
            Action action = () => new OpenApiService(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("swaggerProvider");
        }
        [TestMethod]
        public void GetOpenApiDocument_WithNullForDocumentName_ThrowsException()
        {
            var sut = new OpenApiService(_swaggerProvider);

            Action action = () => sut.GetOpenApiDocument(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("documentName");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        [DataRow("   ")]
        public void GetOpenApiDocument_WithInvalidDataForDocumentName_ThrowsException(string documentName)
        {
            var sut = new OpenApiService(_swaggerProvider);

            Action action = () => sut.GetOpenApiDocument(documentName);

            action.Should().Throw<ArgumentException>().And.ParamName.Should().Be("documentName");
        }

        [TestMethod]
        public void GetOpenApiDocument_PassesOnDocumentName()
        {
            var documentName = "myDocumentName";
            var openApiDocument = new OpenApiDocument();

            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).Returns(openApiDocument);

            var sut = new OpenApiService(_swaggerProvider);

            var document = sut.GetOpenApiDocument(documentName);

            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, null, null)).MustHaveHappenedOnceExactly();
            document.Should().NotBeNull();
        }

        [TestMethod]
        public void GetOpenApiDocument_ReturnsCorrectDocument()
        {
            const string documentName = "myDocumentName";
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

            var sut = new OpenApiService(_swaggerProvider);

            var document = sut.GetOpenApiDocument(documentName);

            document.Should().BeEquivalentTo(openApiDocument);
        }

        [DataTestMethod]
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
        public void GetOpenApiDocument_ForHostAndBasePath_PassesOnCorrectHostAndBasePath(string host, string basePath, string expectedHost, string expectedBasePath)
        {
            const string documentName = "myDocumentName";
            var openApiDocument = new OpenApiDocument();
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, A<string>._, A<string>._)).Returns(openApiDocument);

            var sut = new OpenApiService(_swaggerProvider);

            var document = sut.GetOpenApiDocument(documentName,host, basePath);

            document.Should().NotBeNull();
            A.CallTo(() => _swaggerProvider.GetSwagger(documentName, expectedHost, expectedBasePath)).MustHaveHappenedOnceExactly();
        }
    }
}