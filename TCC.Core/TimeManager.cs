using System;
using System.Linq;
using System.Windows.Threading;
using TCC.Data;
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
        private const DayOfWeek NaResetDay = DayOfWeek.Tuesday;
        private const DayOfWeek RuResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek TwResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek JpResetDay = DayOfWeek.Wednesday;
        private const DayOfWeek KrResetDay = DayOfWeek.Wednesday;

        public const double SecondsInDay = 60 * 60 * 24;

        private static TimeManager _instance;
        private DayOfWeek _resetDay;
        public int ResetHour;
        public static TimeManager Instance => _instance ?? (_instance = new TimeManager());
        public string CurrentRegion;
        private int _serverHourOffset;

        public DateTime CurrentServerTime => DateTime.Now.AddHours(_serverHourOffset);

        private TimeManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            var s = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            s.Tick += CheckNewDay;
            s.Start();
        }

        private void CheckCloseEvents()
        {
            var closeEventsCount = InfoWindowViewModel.Instance.EventGroups.Count(evGroup => evGroup.Events.Any(x => x.IsClose));
            if (closeEventsCount == 0) return;
            WindowManager.FloatingButton.StartNotifying(closeEventsCount);

        }

        private void CheckNewDay(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
                InfoWindowViewModel.Instance.LoadEvents(DateTime.Now.DayOfWeek, CurrentRegion);
            if (DateTime.Now.Second == 0 && DateTime.Now.Minute % 2 == 0) CheckCloseEvents();
        }

        public void SetServerTimeZone(string region)
        {
            if (region == null) return;
            CurrentRegion = region.StartsWith("EU") ? "EU" : region;
            SettingsManager.LastRegion = CurrentRegion;
            var timezone = TimeZoneInfo.Local;
            if (region.StartsWith("EU"))
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == EuTimezone);
                ResetHour = EuResetHour;
                _resetDay = EuResetDay;
            }
            else if (region == "NA")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == NaTimezone);
                ResetHour = NaResetHour;
                _resetDay = NaResetDay;
            }
            else if (region == "RU")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == RuTimezone);
                ResetHour = RuResetHour;
                _resetDay = RuResetDay;
            }
            else if (region == "TW")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == TwTimezone);
                ResetHour = TwResetHour;
                _resetDay = TwResetDay;
            }
            else if (region == "JP")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == JpTimezone);
                ResetHour = JpResetHour;
                _resetDay = JpResetDay;
            }
            else if (region == "KR")
            {
                timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == KrTimezone);
                ResetHour = KrResetHour;
                _resetDay = KrResetDay;
            }
            var serverUtcOffset = timezone.IsDaylightSavingTime(DateTime.UtcNow + timezone.BaseUtcOffset)
                ? timezone.BaseUtcOffset.Hours + 1
                : timezone.BaseUtcOffset.Hours;
            _serverHourOffset = -TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + serverUtcOffset;

            if (InfoWindowViewModel.Instance.Markers.FirstOrDefault(x => x.Name.Equals(region + " server time")) == null)
            {
                InfoWindowViewModel.Instance.Markers.Add(new TimeMarker(_serverHourOffset, region + " server time"));
            }

            CheckReset();
            InfoWindowViewModel.Instance.LoadEvents(DateTime.Now.DayOfWeek, region);

        }

        private void CheckReset()
        {
            if (CurrentRegion == null) return;
            if (SettingsManager.LastRun.Hour >= ResetHour + _serverHourOffset ||
                DateTime.Now.Hour <= ResetHour + _serverHourOffset) return;
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

                if (DateTime.Now.DayOfWeek == _resetDay)
                {
                    ch.WeekliesDone = 0;
                }
            }
            SettingsManager.LastRun = DateTime.Now;
            InfoWindowViewModel.Instance.SaveToFile();
            SettingsManager.SaveSettings();

            ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System",
                "<FONT>Daily/weekly data has been reset.</FONT>"));
        }

    }
}
