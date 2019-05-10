using System.Windows;

namespace TCC.Utilities.Extensions
{
    public static class SizeExtensions
    {
        public static bool IsEqual(this Size s, Size other)
        {
            return s.Width == other.Width &&
                   s.Height == other.Height;
        }
    }
}
