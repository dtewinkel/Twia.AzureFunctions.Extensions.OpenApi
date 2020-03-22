using System;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public abstract class IgnoreParameterAttribute : Attribute
    {
        protected IgnoreParameterAttribute(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name);

            Name = name;
        }

        public string Name { get; }
    }
}