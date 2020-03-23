using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twia.AzureFunctions.Extensions.OpenApi.Documentation;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests.Documentation
{
    [TestClass]
    public class QueryParameterAttributeTests
    {
        private const string _name = "my name";

        [TestMethod]
        public void Constructor_WithNullForName_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new QueryParameterAttribute(null);

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("name");
        }


        [TestMethod]
        public void Constructor_SetsName()
        {
            var sut = new QueryParameterAttribute(_name);

            sut.Name.Should().Be(_name);
        }
    }
}