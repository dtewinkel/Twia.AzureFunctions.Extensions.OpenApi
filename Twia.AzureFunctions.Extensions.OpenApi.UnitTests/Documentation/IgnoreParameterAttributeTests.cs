using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests.Documentation
{
    [TestClass]
    public class IgnoreParameterAttributeTests
    {
        private const string _name = "my name";

        private class ProxyToIgnoreParameterAttribute : IgnoreParameterAttribute
        {
            public ProxyToIgnoreParameterAttribute(string name) : base(name)
            {
            }
        }

        [TestMethod]
        public void Constructor_WithNullForName_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ProxyToIgnoreParameterAttribute(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("name");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void Constructor_WithEmptyOrWhitespaceForName_ThrowsException(string name)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ProxyToIgnoreParameterAttribute(name);

            action.Should().Throw<ArgumentException>().And.ParamName.Should().Be("name");
        }

        [TestMethod]
        public void Constructor_SetsName()
        {
            var sut = new ProxyToIgnoreParameterAttribute(_name);

            sut.Name.Should().Be(_name);
        }
    }
}