using System;
using System.Reflection;
using CommonsLib_DAL.Utils;

namespace CommonsLib_DAL.Extensions
{
    public static class PropertyInfoExtensions
    {

        /// <summary>
        /// Gets given attribute from property info if it exists.
        /// </summary>
        /// <param name="propertyInfo">self</param>
        /// <typeparam name="TAttribute">Attribute to find.</typeparam>
        /// <returns>Found attribute or default.</returns>
        public static TAttribute? GetAttribute<TAttribute>(this PropertyInfo propertyInfo)
            where TAttribute : Attribute
        {
            return ObjectAttributesUtils.GetAttributeFromPropertyInfo<TAttribute>(propertyInfo);
        }
    }
}