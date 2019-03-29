using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TCC
{
    public static class ByteArrayExtensions
    {
        public static string ToStringEx(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }

    public static class ByteExtensions
    {
        public static string ToStringEx(this byte b)
        {
            return $"{b:x2}";
        }
    }
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

    //public static class StringUtils
    //{
    //    public static long Hex8BStringToInt(string hex)
    //    {
    //        var sb = new StringBuilder();
    //        for (var i = 16 - 2; i >= 0; i -= 2)
    //        {
    //            sb.Append(hex[i]);
    //            sb.Append(hex[i + 1]);
    //        }
    //        var result = Convert.ToInt64(sb.ToString(), 16);
    //        return result;
    //    }
    //    public static float Hex4BStringToFloat(string hex)
    //    {
    //        var sb = new StringBuilder();
    //        for (var i = 8 - 2; i >= 0; i -= 2)
    //        {
    //            sb.Append(hex[i]);
    //            sb.Append(hex[i + 1]);
    //        }
    //        var num = UInt32.Parse(sb.ToString(), NumberStyles.AllowHexSpecifier);
    //        var floatVals = BitConverter.GetBytes(num);
    //        var result = BitConverter.ToSingle(floatVals, 0);
    //        return result;
    //    }

    //    public static int Hex4BStringToInt(string hex)
    //    {
    //        var sb = new StringBuilder();
    //        for (var i = 8 - 2; i >= 0; i -= 2)
    //        {
    //            sb.Append(hex[i]);
    //            sb.Append(hex[i + 1]);
    //        }
    //        var result = Convert.ToInt32(sb.ToString(), 16);
    //        return result;
    //    }
    //    public static int Hex2BStringToInt(string hex)
    //    {
    //        var sb = new StringBuilder();
    //        for (var i = 4 - 2; i >= 0; i -= 2)
    //        {
    //            sb.Append(hex[i]);
    //            sb.Append(hex[i + 1]);
    //        }
    //        var result = Convert.ToInt32(sb.ToString(), 16);
    //        return result;
    //    }
    //    public static int Hex1BStringToInt(string hex)
    //    {
    //        var sb = new StringBuilder();
    //        sb.Append(hex[0]);
    //        sb.Append(hex[1]);
    //        var result = Convert.ToInt32(sb.ToString(), 16);
    //        return result;
    //    }
    //    public static int GetStringEnd(string s, int startIndex, string terminator)
    //    {
    //        var endIndex = startIndex;
    //        var zeroes = false;
    //        while (!zeroes)
    //        {
    //            if (endIndex + 3 <= s.Length)
    //            {
    //                var test = s[endIndex + 0].ToString() +
    //                              s[endIndex + 1].ToString() +
    //                              s[endIndex + 2].ToString() +
    //                              s[endIndex + 3].ToString();

    //                if (test == terminator)
    //                {
    //                    zeroes = true;
    //                }
    //                else
    //                {
    //                    endIndex += 4;
    //                }
    //            }
    //            else { return startIndex; }
    //        }
    //        return endIndex;
    //    }

    //    public static string GetStringFromHex(string hex, int startIndex, string terminator)
    //    {
    //        var builder = new StringBuilder();
    //        for (var i = startIndex; i < GetStringEnd(hex, startIndex, terminator); i += 2)
    //        {
    //            builder.Append(hex[i].ToString() + hex[i + 1].ToString());
    //        }
    //        builder.Replace("00", "");
    //        var b = builder.ToString().ToByteArray();
    //        return Encoding.UTF7.GetString(b);
    //    }
    //}

}
