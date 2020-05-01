using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi.UnitTests
{
    internal static class FunctionMethodTestSource
    {
        public static void JustAMethod()
        {
            // Just for testing.
        }

        public static MethodInfo GetMethodInfo()
        {
            return typeof(FunctionMethodTestSource).GetMethod(nameof(FunctionMethodTestSource.JustAMethod));
        }
    }
}