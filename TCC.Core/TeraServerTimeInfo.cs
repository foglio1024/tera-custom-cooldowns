using System;

namespace TCC
{
    public struct TeraServerTimeInfo
    {
        public readonly string Timezone;
        public readonly int ResetHour;
        public readonly DayOfWeek VanguardResetDay;
        public readonly DayOfWeek DungeonsWeeklyResetDay;

        public TeraServerTimeInfo(string tz, int rh, DayOfWeek vanguardReset, DayOfWeek dungeonWeeklyReset)
        {
            Timezone = tz;
            ResetHour = rh;
            VanguardResetDay = vanguardReset;
            DungeonsWeeklyResetDay = dungeonWeeklyReset;
        }
    }
}