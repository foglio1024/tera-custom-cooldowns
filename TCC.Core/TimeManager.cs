using System;
using System.Linq;
using System.Windows.Threading;
using TCC.ViewModels;

namespace TCC
{
    public class TimeManager : TSPropertyChanged
    {
        private const string EuTimezone = "Central Europe Standard Time";
        private const string NaTimezone = "Central Standard Time";
        private const string RuTimezone = "Russian Standard Time";
        private const string TwTimezone = "China Standard Time";
        private const string JpTimezone = "Tokyo Standard Time";
        private const string KrTimezone = "Korea Standard Time";

        private const int EuResetHour = 6;
        private const int NaResetHour = 6;
        private const int RuResetHour = 6;
        private const int TwResetHour = 6;
        private const int JpResetHour = 6;
        private const int KrResetHour = 6;

        private const DayOfWeek EuResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek NaResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek RuResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek TwResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek JpResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek KrResetDay = DayOfWeek.Wednesday;

        public const double SecondsInDay = 60 * 60 * 24;

        private static TimeManager _instance;
        private DayOfWeek _resetDay;
        public int ResetHour;
        public static TimeManager Instance => _instance ?? (_instance = new TimeManager());
        private readonly DispatcherTimer _t;
        private string _currentRegion;
        public int ServerHourOffset { get; set; }

        public TimeManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _t = new DispatcherTimer();
            _t.Interval = TimeSpan.FromMilliseconds(1000);
            _t.Tick += CheckNewDay;
        }

        private void CheckNewDay(object sender, EventArgs e)
        {
            if(DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
            {
                InfoWindowViewModel.Instance.LoadEvents(DateTime.Now.DayOfWeek, _currentRegion);
            }
        }

        public void SetServerTimeZone(string region)
        {
            _currentRegion = region;
            TimeZoneInfo timezone = TimeZoneInfo.Local;
            if (region.StartsWith("EU"))
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == EuTimezone);
                ResetHour = EuResetHour;
                _resetDay = EuResetDay;
            }
            else switch (region)
            {
                case "NA":
                    timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == NaTimezone);
                    ResetHour = NaResetHour;
                    _resetDay = NaResetDay;
                    break;
                case "RU":
                    timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == RuTimezone);
                    ResetHour = RuResetHour;
                    _resetDay = RuResetDay;
                    break;
                case "TW":
                    timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == TwTimezone);
                    ResetHour = TwResetHour;
                    _resetDay = TwResetDay;
                    break;
                case "JP":
                    timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == JpTimezone);
                    ResetHour = JpResetHour;
                    _resetDay = JpResetDay;
                    break;
                case "KR":
                    timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == KrTimezone);
                    ResetHour = KrResetHour;
                    _resetDay = KrResetDay;
                    break;
            }
            var serverUtcOffset = timezone.IsDaylightSavingTime(DateTime.UtcNow + timezone.BaseUtcOffset) ? 
                timezone.BaseUtcOffset.Hours + 1 : 
                timezone.BaseUtcOffset.Hours;
            ServerHourOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours - serverUtcOffset;
            if (ServerHourOffset != 0)
            {
                if (InfoWindowViewModel.Instance.Markers.FirstOrDefault(x => x.Name.Equals(region + " server time")) != null) return;
                InfoWindowViewModel.Instance.Markers.Add(new TimeMarker(DateTime.Now.AddHours(-ServerHourOffset), region + " server time", "fff5c6"));
            }
            var sg = new EventGroup("Special events");
            sg.AddEvent(new DailyEvent("Reset", TimeManager.Instance.ResetHour, 0, "ff0000"));
            InfoWindowViewModel.Instance.AddEventGroup(sg);
            InfoWindowViewModel.Instance.LoadEvents(DateTime.Now.DayOfWeek, region);

        }
        public bool CheckReset()
        {
            if (SettingsManager.LastRun.Hour < ResetHour + ServerHourOffset && DateTime.Now.Hour > ResetHour + ServerHourOffset)
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

                    if(DateTime.Now.DayOfWeek == _resetDay)
                    {
                        ch.WeekliesDone = 0;
                    }
                }
                SettingsManager.LastRun = DateTime.Now;
                InfoWindowViewModel.Instance.SaveToFile();
                SettingsManager.SaveSettings();
                return true;
            }
            else return false;
        }

    }
}
