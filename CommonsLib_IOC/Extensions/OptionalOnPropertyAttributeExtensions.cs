using System;
using CommonsLib_DAL.Attributes;
using Microsoft.Extensions.Configuration;

namespace CommonsLib_IOC.Extensions
{
    /// <summary>
    /// Extensions for OptionalOnPropertyAttribute.
    /// </summary>
    public static class OptionalOnPropertyAttributeExtensions
    {
        /// <summary>
        /// Validates whether or not property matches.
        /// </summary>
        /// <param name="attribute">Current Attribute</param>
        /// <param name="configuration">Configuration to validate property.</param>
        /// <returns></returns>
        public static bool Validate(this OptionalOnPropertyAttribute attribute, IConfiguration configuration)
        {
            var value = configuration.GetValue<string?>(attribute.Property);
            if (attribute.MatchIfMissing && value == null)
                return true;
            return value.Equals(attribute.HavingValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}