using System;
using System.Globalization;
using UIKit;

namespace CommonsLib_iOS.Helper
{
    public class UiHelper
    {

        /// <summary>
        /// Tries to parse given hexString as a UIColor.
        /// </summary>
        /// <param name="hexString">Hex string to parse.</param>
        /// <returns>Built UIColor</returns>
        /// <exception cref="ArgumentException">If given hex string format is invalid.</exception>
        public static UIColor FromHex(string hexString)
        {
            var colorString = hexString.Replace("#", string.Empty);
            nfloat GetDecimalValueFrom(string str)
            {
                var fullHex = str.Length == 2 ? str : $"{str}{str}";
                var hexComponent = int.Parse(fullHex, NumberStyles.HexNumber);
                return new nfloat(hexComponent / 255.0);
            }
            nfloat alpha, red, blue, green;
            switch (colorString.Length)
            {
                case 3:
                    alpha = 1.0f;
                    red = GetDecimalValueFrom(colorString.Substring(0, 1));
                    green = GetDecimalValueFrom(colorString.Substring(1, 1));
                    blue = GetDecimalValueFrom(colorString.Substring(2, 1));
                    break;
                case 4: 
                    alpha = GetDecimalValueFrom(colorString.Substring(0, 1));
                    red = GetDecimalValueFrom(colorString.Substring(1, 1));
                    green = GetDecimalValueFrom(colorString.Substring(2, 1));
                    blue = GetDecimalValueFrom(colorString.Substring(3, 1));
                    break;
                case 6: 
                    alpha = 1.0f;
                    red = GetDecimalValueFrom(colorString.Substring(0, 2));
                    green = GetDecimalValueFrom(colorString.Substring(2, 2));
                    blue = GetDecimalValueFrom(colorString.Substring(4, 2));
                    break;
                case 8:
                    alpha = GetDecimalValueFrom(colorString.Substring(0, 2));
                    red = GetDecimalValueFrom(colorString.Substring(2, 2));
                    green = GetDecimalValueFrom(colorString.Substring(4, 2));
                    blue = GetDecimalValueFrom(colorString.Substring(6, 2));
                    break;
                default:
                    throw new ArgumentException($"Invalid hexString {hexString}, " +
                                                "hexString should be a hex value of the form " +
                                                "#RBG, #ARGB, #RRGGBB, or #AARRGGBB");
            }
            return new UIColor(red, green, blue, alpha);
        }

    }
}