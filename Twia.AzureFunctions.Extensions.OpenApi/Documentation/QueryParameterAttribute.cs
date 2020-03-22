﻿using System;
using EnsureThat;

namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, Inherited = true, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        private Type _type = typeof(string);

        public QueryParameterAttribute(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name);

            Name = name;
        }

        public string Name { get; }

        public Type Type
        {
            get => _type;

            set
            {
                EnsureArg.IsNotNull(value, nameof(value));
                EnsureArg.IsNotOfType(Type, typeof(void), nameof(value));

                _type = value;
            }
        }

        public string Description { get; set; }

        public bool IsRequired { get; set; } = true;
    }
}