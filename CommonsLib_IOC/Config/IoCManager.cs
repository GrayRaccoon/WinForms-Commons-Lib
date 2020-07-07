namespace CommonsLib_IOC.Config
{
    /// <summary>
    /// Abstract IoC Manager Class.
    /// IResolver Instance must be initialized in the highest app's layer.
    /// </summary>
    public static class IoCManager
    {
        /// <summary>
        /// Global IResolver Instance.
        /// </summary>
        public static IResolver Resolver { get; set; }
    }
}