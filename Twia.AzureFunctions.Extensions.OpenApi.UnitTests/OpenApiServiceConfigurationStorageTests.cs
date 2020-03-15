using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class OpenApiServiceConfigurationStorageTests
    {
        [TestMethod]
        public void Constructor_WithNullForFunctionAssembly_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new OpenApiServiceConfigurationStorage(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("functionAssembly");
        }

        [TestMethod]
        public void Constructor_WithFunctionAssembly_SetsFunctionAssembly()
        {
            var functionAssembly = typeof(string).Assembly;
            var sut = new OpenApiServiceConfigurationStorage(functionAssembly);

            sut.FunctionAssembly.Should().BeSameAs(functionAssembly);
        }
    }
}