using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<string> GetXmlFilePaths(this Assembly functionAssembly)
        {
            EnsureArg.IsNotNull(functionAssembly, nameof(functionAssembly));

            var pathsToTest = new List<string>();
            var inPath = Path.GetDirectoryName(functionAssembly.Location);
            pathsToTest.Add(inPath);
            var currentDirectory = Environment.CurrentDirectory;
            if (currentDirectory != inPath)
            {
                pathsToTest.Add(currentDirectory);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var dllFiles = Directory.EnumerateFiles(inPath, "*.dll", SearchOption.TopDirectoryOnly);

            return dllFiles.Select(Path.GetFileNameWithoutExtension)
                .Select(baseName => $"{baseName}.xml")
                .Select(fileName => pathsToTest
                    .Select(pathToTest => Path.Combine(pathToTest, fileName))
                    .FirstOrDefault(File.Exists))
                .Where(xmlFile => xmlFile != default);
        }
    }
}