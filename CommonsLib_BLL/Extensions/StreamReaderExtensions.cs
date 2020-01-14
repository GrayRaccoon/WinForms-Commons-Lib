using System.Collections.Generic;
using System.IO;

namespace CommonsLib_BLL.Extensions
{
    /// <summary>
    /// StreamReader Extensions.
    /// </summary>
    public static class StreamReaderExtensions
    {
        /// <summary>
        /// Read all lines from an open StreamReader.
        /// </summary>
        /// <param name="self">StreamReader itself.</param>
        /// <returns>All lines from stream reader.</returns>
        public static IEnumerable<string> ReadAllLines(this StreamReader self)
        {
            string line;
            while ((line = self.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}