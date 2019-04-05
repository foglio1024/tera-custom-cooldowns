namespace TCC.Utilities.Extensions
{
    public static class ByteExtensions
    {
        public static string ToStringEx(this byte b)
        {
            return $"{b:x2}";
        }
    }
}