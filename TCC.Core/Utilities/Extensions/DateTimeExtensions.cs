using System;

namespace TCC.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToEpoch(this DateTime dt)
        {
            var t = dt - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }
    }
}