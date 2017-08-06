using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.ViewModels;

namespace TCC
{
    public class TimeManager : TSPropertyChanged
    {
        const string EU_TIMEZONE = "Central Europe Standard Time";
        const string NA_TIMEZONE = "Central Standard Time";
        const string RU_TIMEZONE = "Russian Standard Time";
        const string TW_TIMEZONE = "China Standard Time";
        const string JP_TIMEZONE = "Japan Standard Time";
        const string KR_TIMEZONE = "Korea Standard Time";

        const int EU_RESET_HOUR = 6;
        const int NA_RESET_HOUR = 6;
        const int RU_RESET_HOUR = 6;
        const int TW_RESET_HOUR = 6;
        const int JP_RESET_HOUR = 6;
        const int KR_RESET_HOUR = 6;

        const DayOfWeek EU_RESET_DAY = DayOfWeek.Wednesday;
        const DayOfWeek NA_RESET_DAY = DayOfWeek.Wednesday;
        const DayOfWeek RU_RESET_DAY = DayOfWeek.Wednesday;
        const DayOfWeek TW_RESET_DAY = DayOfWeek.Wednesday;
        const DayOfWeek JP_RESET_DAY = DayOfWeek.Wednesday;
        const DayOfWeek KR_RESET_DAY = DayOfWeek.Wednesday;

        static TimeManager _instance;
        DayOfWeek resetDay;
        int resetHour;
        int serverHourOffset;
        public static TimeManager Instance => _instance ?? (_instance = new TimeManager());


        public TimeManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        public void SetServerTimeZone(string region)
        {
            TimeZoneInfo timezone = TimeZoneInfo.Local;
            if (region.StartsWith("EU"))
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == EU_TIMEZONE);
                resetHour = EU_RESET_HOUR;
                resetDay = EU_RESET_DAY;
            }
            else if (region == "NA")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == NA_TIMEZONE);
                resetHour = NA_RESET_HOUR;
                resetDay = NA_RESET_DAY;

            }
            else if (region == "RU")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == RU_TIMEZONE);
                resetHour = RU_RESET_HOUR;
                resetDay = RU_RESET_DAY;

            }
            else if (region == "TW")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == TW_TIMEZONE);
                resetHour = TW_RESET_HOUR;
                resetDay = TW_RESET_DAY;

            }
            else if (region == "JP")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == JP_TIMEZONE);
                resetHour = JP_RESET_HOUR;
                resetDay = JP_RESET_DAY;

            }
            else if (region == "KR")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == KR_TIMEZONE);
                resetHour = KR_RESET_HOUR;
                resetDay = KR_RESET_DAY;

            }
            var serverUtcOffset = timezone.IsDaylightSavingTime(DateTime.UtcNow + timezone.BaseUtcOffset) ? 
                timezone.BaseUtcOffset.Hours + 1 : 
                timezone.BaseUtcOffset.Hours;
            serverHourOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours - serverUtcOffset;
        }
        public bool CheckReset()
        {
            if (SettingsManager.LastRun.Hour < resetHour + serverHourOffset && DateTime.Now.Hour > resetHour + serverHourOffset)
            {
                foreach (var ch in InfoWindowViewModel.Instance.Characters)
                {
                    foreach (var dg in ch.Dungeons)
                    {
                        if (dg.Id == 9950)
                        {
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday) dg.Reset();
                            else continue;
                        }
                        dg.Reset();
                    }
                    ch.DailiesDone = 0;

                    if(DateTime.Now.DayOfWeek == resetDay)
                    {
                        ch.WeekliesDone = 0;
                    }
                }
                SettingsManager.LastRun = DateTime.Now;
                SettingsManager.SaveSettings();
                return true;
            }
            else return false;
        }

    }
}
