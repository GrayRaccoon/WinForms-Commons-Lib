using System;
using CommonsLib_DAL.Utils;

namespace CommonsLib_DAL.Extensions
{
    /// <summary>
    /// Some utility extensions related to Custom Attributes for all object types.
    /// </summary>
    public static class ObjectAttributesExtensions
    {

        /// <summary>
        /// Gets the value of the first Type property that contains the provided attribute.
        /// </summary>
        /// <param name="instance">Instance where to extract the value.</param>
        /// <typeparam name="TObject">Expected property return type.</typeparam>
        /// <typeparam name="TAttribute">Expected Attribute type</typeparam>
        /// <returns>Found property value for the attribute.</returns>
        public static TObject GetValueForAttribute<TObject, TAttribute>(
            this object instance)
            where TAttribute : Attribute
        {
            var propInfo = ObjectAttributesUtils.GetPropertyInfoForAttribute<TAttribute>(instance.GetType());
            var value = propInfo?.GetValue(instance);
            return value is TObject val ? val : default;
        }

        /// <summary>
        /// Sets the value of the first Type property that contains the provided attribute.
        /// </summary>
        /// <param name="instance">Instance where to insert the value.</param>
        /// <param name="value">Value to be set in the found property.</param>
        /// <typeparam name="TAttribute">Expected Attribute type</typeparam>
        public static void SetValueForAttribute<TAttribute>(
            this object instance, object value)
            where TAttribute : Attribute
        {
            var propInfo = ObjectAttributesUtils.GetPropertyInfoForAttribute<TAttribute>(instance.GetType());
            propInfo?.SetValue(instance, value);
        }

        /// <summary>
        /// Gets the property name of the first property that contains the provided attribute.
        /// </summary>
        /// <param name="instance">Instance where to extract the value.</param>
        /// <typeparam name="TAttribute">Expected Attribute type</typeparam>
        /// <returns>Found property name for the attribute.</returns>
        public static string GetPropertyNameForAttribute<TAttribute>(
            this object instance)
            where TAttribute : Attribute
        {
            var propInfo = ObjectAttributesUtils.GetPropertyInfoForAttribute<TAttribute>(instance.GetType());
            return propInfo?.Name ?? string.Empty;
        }
        
    }
}