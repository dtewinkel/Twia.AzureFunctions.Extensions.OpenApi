using System;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class IgnoreHeaderParameterAttribute : Attribute
    {
        public IgnoreHeaderParameterAttribute(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name);

            Name = name;
        }

        public string Name { get; }
    }
}