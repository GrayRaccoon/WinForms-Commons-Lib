using System;

namespace CommonsLib_DAL.Attributes
{
    /// <summary>
    /// Attribute to identify whether or not certain component is optional
    /// based on the value of certain property
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionalOnPropertyAttribute: Attribute
    {
        public string Property { get; set; } = string.Empty;
        public string HavingValue { get; set; } = "true"; 
        public bool MatchIfMissing { get; set; } = false;
    }
}