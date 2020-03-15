using System;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    /// <summary>
    /// Tag a method or parameter to be ignored for the Open API Documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class OpenApiIgnoreAttribute : Attribute
    {
    }
}