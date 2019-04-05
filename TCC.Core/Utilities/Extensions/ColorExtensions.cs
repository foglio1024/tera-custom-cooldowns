using System.Windows.Media;

namespace TCC.Utilities.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color col, bool alpha = false)
        {
            return $"#{(alpha ? col.A.ToStringEx() : "")}{col.R.ToStringEx()}{col.G.ToStringEx()}{col.B.ToStringEx()}";
        }
    }
}