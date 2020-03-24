using System;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public abstract class ParameterAttribute : Attribute
    {
        private Type _type = typeof(string);

        protected ParameterAttribute(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public string Name { get; }

        public Type Type
        {
            get => _type;

            set
            {
                EnsureArg.IsNotNull(value, nameof(value));
                EnsureArg.IsNotOfType(value, typeof(void), nameof(value));

                _type = value;
            }
        }

        public string Description { get; set; }

        public bool IsRequired { get; set; } = true;
    }
}