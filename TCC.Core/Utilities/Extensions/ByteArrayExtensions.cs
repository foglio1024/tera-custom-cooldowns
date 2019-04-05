using System.Text;

namespace TCC.Utilities.Extensions
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
}
