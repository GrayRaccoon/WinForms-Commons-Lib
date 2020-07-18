using System;
using Android.Content;
using Newtonsoft.Json;

namespace CommonsLib_Droid.Extensions
{
    public static class IntentExtensions
    {
        
        /// <summary>
        /// Puts an object as a its string value in the intent extras.
        /// </summary>
        /// <param name="intent">self</param>
        /// <param name="key">Extra key</param>
        /// <param name="value">Extra object value</param>
        public static void PutObjectExtra(this Intent intent, string key, object value)
        {
            var paramStr = JsonConvert.SerializeObject(value);
            intent.PutExtra(key, paramStr);
        }

        /// <summary>
        /// Gets an object value from a intent extra.
        /// </summary>
        /// <param name="intent">self</param>
        /// <param name="key">Extra key</param>
        /// <param name="outType">Expected extra type</param>
        /// <returns>Found extra object value</returns>
        public static object GetObjectExtra(this Intent intent, string key, Type outType)
        {
            var paramStr = intent.GetStringExtra(key);
            return JsonConvert.DeserializeObject(paramStr, outType);
        }
        
    }
}