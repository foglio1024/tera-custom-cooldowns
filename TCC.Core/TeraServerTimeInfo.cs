using System;

namespace TCC
{
    public struct TeraServerTimeInfo
    {
        public readonly string Timezone;
        public readonly int ResetHour;
        public readonly DayOfWeek ResetDay;

        public TeraServerTimeInfo(string tz, int rh, DayOfWeek rd)
        {
            Timezone = tz;
            ResetHour = rh;
            ResetDay = rd;
        }
    }
}