using System.IO;
using System.Linq;
using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    public static class AssemblyHelper
    {

        public static Assembly GetFunctionAssembly(string fromPath, string searchPattern = "*.Function.dll")
        {
            var dllFile = Directory
                .EnumerateFiles(fromPath, searchPattern, SearchOption.AllDirectories)
                .Select(Path.GetFullPath)
                .OrderBy(file => new FileInfo(file).LastAccessTimeUtc)
                .Last();

#pragma warning disable S3885 // Remove unused parameter
            return Assembly.LoadFrom(dllFile);
#pragma warning restore S3885 // Remove unused parameter
        }
    }
}