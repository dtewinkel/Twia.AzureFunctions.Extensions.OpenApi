using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests.Documentation
{
    [TestClass]
    public class ParameterAttributeTests
    {
        private const string _name = "my name";

        private class ProxyToParameterAttribute : ParameterAttribute
        {
            public ProxyToParameterAttribute(string name) : base(name)
            {
            }
        }

        [TestMethod]
        public void Constructor_WithNullForName_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ProxyToParameterAttribute(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("name");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void Constructor_WithEmptyOrWhitespaceForName_ThrowsException(string name)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ProxyToParameterAttribute(name);

            action.Should().Throw<ArgumentException>().And.ParamName.Should().Be("name");
        }

        [TestMethod]
        public void Constructor_SetsName()
        {
            var sut = new ProxyToParameterAttribute(_name);

            sut.Name.Should().Be(_name);
        }

        [TestMethod]
        public void Constructor_SetsIsRequiredToTrue()
        {
            var sut = new ProxyToParameterAttribute(_name);

            sut.IsRequired.Should().BeTrue();
        }

        [TestMethod]
        public void IsRequired_WhenSetToFalse_ReturnsFalse()
        {
            var sut = new ProxyToParameterAttribute(_name)
            {
                IsRequired = false
            };

            sut.IsRequired.Should().BeFalse();
        }

        [TestMethod]
        public void Constructor_SetsTypeToString()
        {
            var sut = new ProxyToParameterAttribute(_name);

            sut.Type.Should().Be(typeof(string));
        }

        [TestMethod]
        public void Type_WhenSetToNull_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var sut = new ProxyToParameterAttribute(_name);

            Action action = () => sut.Type = null;

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [TestMethod]
        public void Type_WhenSetToInt_ReturnsInt()
        {
            var type = typeof(int);
            var sut = new ProxyToParameterAttribute(_name)
            {
                Type = type
            };

            sut.Type.Should().Be(type);
        }

        [TestMethod]
        public void Description_WhenSet_ReturnsValue()
        {
            const string description = "A nice description.";

            var sut = new ProxyToParameterAttribute(_name)
            {
                Description = description
            };

            sut.Description.Should().Be(description);
        }
    }
}