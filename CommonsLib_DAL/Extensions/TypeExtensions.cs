using System;
using CommonsLib_DAL.Utils;

namespace CommonsLib_DAL.Extensions
{
    /// <summary>
    /// Some utility extensions related to Custom Attributes for Type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Detects whether or not given type has given attribute.
        /// </summary>
        /// <param name="type">type to validate.</param>
        /// <typeparam name="TAttribute">Attribute to find</typeparam>
        /// <returns>Whether or not type has attribute</returns>
        public static bool HasAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return ObjectAttributesUtils.HasAttribute<TAttribute>(type);
        }

        /// <summary>
        /// Gets a given attribute for a given class if it exists, otherwise returns null.
        /// </summary>
        /// <param name="type">type to validate</param>
        /// <typeparam name="TAttribute">Attribute to find.</typeparam>
        /// <returns>Found Attribute if exists</returns>
        public static TAttribute? GetAttributeIfExists<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return ObjectAttributesUtils.GetAttributeFor<TAttribute>(type);
        }

        /// <summary>
        /// Checks whether given type is same of parent of current type. 
        /// </summary>
        /// <param name="type">type to validate</param>
        /// <param name="baseType">Potential base type.</param>
        /// <returns>Whether or not given type is subtype of this type.</returns>
        public static bool IsSameOrSubclassOf(this Type type, Type baseType)
        {
            return type == baseType || type.IsSubclassOf(baseType);
        }
    }
}