using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    [TestClass]
    public class AssemblyExtensionsTests
    {
        [TestMethod]
        public void GetXmlFilePaths_OnNullForFunctionAssembly_ThrowsException()
        {
            Assembly functionAssembly = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Action action = () => functionAssembly.GetXmlFilePaths();

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("functionAssembly");
        }

        [TestMethod]
        public void GetXmlFilePaths_OnTestAssembly_ReturnsXmlFiles()
        {
            var originalCurrentDirectory = Environment.CurrentDirectory;
            var expectedXmlFiles = new[] {"bin\\TestAssembly.Client.xml", "TestAssembly.Function.xml"};

            try
            {
                var functionAssembly = AssemblyHelper.GetFunctionAssembly("../../../../TestAssembly.Function/bin");

                var fileInfo = new FileInfo(functionAssembly.Location);
                var directory = fileInfo.Directory.Parent;
                Environment.CurrentDirectory = directory.FullName;

                var xmlFilePaths = functionAssembly.GetXmlFilePaths();

                xmlFilePaths.Should().NotBeEmpty();
                xmlFilePaths.Should()
                    .BeEquivalentTo(expectedXmlFiles.Select(file => Path.Combine(directory.FullName, file)));
            }
            finally
            {
                Environment.CurrentDirectory = originalCurrentDirectory;
            }
        }

    }
}