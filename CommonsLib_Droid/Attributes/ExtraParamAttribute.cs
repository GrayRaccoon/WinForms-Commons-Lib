using System;

namespace CommonsLib_Droid.Attributes
{
    /// <summary>
    /// Attribute to identify Activity properties that might fetch its value from intent extras.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExtraParamAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;
    }
}