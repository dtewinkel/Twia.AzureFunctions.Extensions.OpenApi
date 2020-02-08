using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class SwaggerServiceConfigurationStorageTests
    {
        [TestMethod]
        public void Constructor_WithNullForFunctionAssembly_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new SwaggerServiceConfigurationStorage(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("functionAssembly");
        }

        [TestMethod]
        public void SwaggerServiceConfigurationStorage_WithFunctionAssembly_SetsFunctionAssembly()
        {
            var functionAssembly = typeof(string).Assembly;
            var sut = new SwaggerServiceConfigurationStorage(functionAssembly);

            sut.FunctionAssembly.Should().BeSameAs(functionAssembly);
        }
    }
}