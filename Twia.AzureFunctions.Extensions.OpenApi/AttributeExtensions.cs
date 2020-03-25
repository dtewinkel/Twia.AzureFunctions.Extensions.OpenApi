using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public static class AttributeExtensions
    {
        public static IList<T> GetAttributes<T>(this ICustomAttributeProvider element) where T: Attribute
        {
            return element.GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .ToList();
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider element) where T : Attribute
        {
            return element.GetCustomAttributes(typeof(T), false).Any();
        }

    }
}