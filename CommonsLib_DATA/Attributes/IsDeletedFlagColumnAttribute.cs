using System;

namespace CommonsLib_DATA.Attributes
{
    /// <summary>
    /// Attribute to identify which Field can be used as Is Deleted Flag.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsDeletedFlagColumnAttribute : Attribute
    {
        public bool IsDeletedValue { get; set; } = true;
    }
}