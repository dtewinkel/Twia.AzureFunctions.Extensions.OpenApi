using System;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OpenApiBodyTypeAttribute : Attribute
    {
        public Type Type { get; }


        public OpenApiBodyTypeAttribute(Type type)
        {
            EnsureArg.IsNotNull(type, nameof(type));

            Type = type;
        }
    }
}