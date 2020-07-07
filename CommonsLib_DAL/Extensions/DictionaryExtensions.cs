using System.Collections.Generic;

namespace CommonsLib_DAL.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Finds value in dictionary for given key, if given key does not exists
        /// then it returns provided default value.
        /// </summary>
        /// <param name="dict">Self</param>
        /// <param name="key">Key to find.</param>
        /// <param name="defValue">Default value</param>
        /// <typeparam name="TKey">Dict Key type</typeparam>
        /// <typeparam name="TValue">Dict Value type</typeparam>
        /// <returns>Found value.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defValue)
        {
            return dict.ContainsKey(key) ? dict[key] : defValue;
        }
    }
}