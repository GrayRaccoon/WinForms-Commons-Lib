using System;

namespace CommonsLib_DAL.Attributes
{
    /// <summary>
    /// Attribute to identify class implementation that must be registered into IoC Container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public bool Primary { get; set; } = false;
    }
}