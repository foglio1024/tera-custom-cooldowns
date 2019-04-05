using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TCC.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArrayHex(this string str)
        {
            var numberChars = str.Length / 2;
            var bytes = new byte[numberChars];
            using (var sr = new StringReader(str))
            {
                for (var i = 0; i < numberChars; i++)
                    bytes[i] =
                        Convert.ToByte(new string(new[] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }
        public static byte[] ToByteArray(this string str)
        {
            var ret = new byte[str.Length];
            for (var i = 0; i < str.Length; i++)
            {
                ret[i] = Convert.ToByte(str[i]);
            }
            return ret;
            //return Encoding.Default.GetBytes(str);
        }
        public static string ReplaceHtmlEscapes(this string str)
        {
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&#xA", "\n");
            str = str.Replace("&quot;", "\"");
            return str;
        }
        public static string ReplaceFirstOccurrenceCaseInsensitive(this string input, string search, string replacement)
        {
            var pos = input.IndexOf(search, StringComparison.InvariantCultureIgnoreCase);
            if (pos < 0) return input;
            var result = input.Substring(0, pos) + replacement + input.Substring(pos + search.Length);
            return result;
        }

        public static string ReplaceCaseInsensitive(this string input, string search, string replacement)
        {
            var result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

    }
}