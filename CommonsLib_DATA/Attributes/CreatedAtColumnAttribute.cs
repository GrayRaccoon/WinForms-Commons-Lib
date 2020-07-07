using System;

namespace CommonsLib_DATA.Attributes
{
    /// <summary>
    /// Attribute to identify which Field can be used as Created At datetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CreatedAtColumnAttribute : Attribute
    { }
}