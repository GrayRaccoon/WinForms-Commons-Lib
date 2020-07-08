using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommonsLib_DAL.Utils
{
    /// <summary>
    /// Attributes utility methods for objects.
    /// </summary>
    public static class ObjectAttributesUtils
    {
        /// <summary>
        /// Checks whether or not a type has given Attribute.
        /// </summary>
        /// <param name="type">Type to validate</param>
        /// <typeparam name="TAttribute">Attribute to find</typeparam>
        /// <returns></returns>
        public static bool HasAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var attribute = GetAttributeFor<TAttribute>(type);
            return attribute != null;
        }

        /// <summary>
        /// Tries to find an attribute in a given type.
        /// </summary>
        /// <param name="type">Type to validate</param>
        /// <typeparam name="TAttribute">Attribute to find</typeparam>
        /// <returns>Attribute if exists in type</returns>
        public static TAttribute? GetAttributeFor<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            return type.GetCustomAttributes<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the property name of the first property that contains the provided attribute.
        /// </summary>
        /// <typeparam name="TInstance">Instance Type where to extract the value.</typeparam>
        /// <typeparam name="TAttribute">Expected Attribute type</typeparam>
        /// <returns>Found property name for the attribute.</returns>
        public static string GetPropertyNameForAttribute<TInstance, TAttribute>()
            where TInstance : new()
            where TAttribute : Attribute
        {
            var propInfo = GetPropertyInfoForAttribute<TAttribute>(typeof(TInstance));
            return propInfo?.Name ?? string.Empty;
        }


        /// <summary>
        /// Gets the property info of the first property that contains the provided attribute. 
        /// </summary>
        /// <param name="instanceType">Instance type to extract property info.</param>
        /// <typeparam name="TAttribute">Expected Attribute type</typeparam>
        /// <returns>Found property info for the attribute.</returns>
        public static PropertyInfo? GetPropertyInfoForAttribute<TAttribute>(
            Type instanceType)
            where TAttribute : Attribute
        {
            return (
                from propertyInfo in instanceType.GetProperties()
                let customAttributes = propertyInfo.GetCustomAttributes(typeof(TAttribute), true)
                where customAttributes.Length != 0
                select propertyInfo
            ).FirstOrDefault();
        }

        /// <summary>
        /// Gets the attribute from the property info of the first property that contains the given attribute.
        /// </summary>
        /// <param name="instanceType">Instance type to extract property info attribute.</param>
        /// <typeparam name="TAttribute">Expected attribute type.</typeparam>
        /// <returns>Found property info attribute.</returns>
        public static TAttribute? GetAttributeForAnyFieldIn<TAttribute>(Type instanceType)
            where TAttribute : Attribute
        {
            var propInfo = GetPropertyInfoForAttribute<TAttribute>(instanceType);
            if (propInfo == default)
                return null;
            var cusAttributes = propInfo?.GetCustomAttributes(typeof(TAttribute), true);
            return (TAttribute?) cusAttributes?[0];
        }
    }
}