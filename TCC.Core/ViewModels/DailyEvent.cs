using System;
using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class DailyEvent : TSPropertyChanged
    {
        private DateTime Start { get; set; }
        private TimeSpan Duration { get; set; }
        private readonly TimeSpan _realDuration;
        public double StartFactor => 60 * (Start.Hour * 60 + Start.Minute) / TimeManager.SecondsInDay;
        public double DurationFactor => Duration.TotalSeconds / TimeManager.SecondsInDay;
        private bool _happened;
        public bool IsClose
        {
            get
            {
                var ts = Start - TimeManager.Instance.CurrentServerTime;
                return ts.TotalMinutes > 0 && ts.TotalMinutes <= 15;
            }
        }
        public string Name { get; set; }
        public string ToolTip
        {
            get
            {
                var d = Duration > TimeSpan.FromHours(0) ? " to " + Start.Add(_realDuration).ToShortTimeString() : "";
                return Name + " " + Start.ToShortTimeString() + d;
            }
        }
        public string Color { get; }
        public DailyEvent(string name, double startHour, double startMin, double durationOrEndHour, string color = "30afff", bool isDuration = true)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //var m = startHour % 1;
            //var h = startHour - m;
            Start = DateTime.Parse(startHour + ":"+startMin);
            var d = isDuration ? durationOrEndHour : durationOrEndHour - startHour;
            Duration = TimeSpan.FromHours(d);
            _realDuration = Duration;
            var dayend = DateTime.Parse("00:00").AddDays(1);
            if (Start.Add(Duration) > dayend)
            {
                Duration = dayend - Start;
            }
            Name = name;
            Color = color;
        }

        public void UpdateFromServer(bool force = false)
        {
            if (TimeManager.Instance.CurrentServerTime >= Start && !_happened)
            {
                var time = force
                    ? TimeManager.Instance.CurrentServerTime
                    : TimeManager.Instance.RetrieveGuildBamDateTime().AddHours(TimeManager.Instance.ServerHourOffsetFromUtc);

                if (time >= Start)
                {
                    Start = time;
                    Duration = TimeSpan.Zero;
                    _happened = true;
                    NPC(nameof(StartFactor));
                    NPC(nameof(DurationFactor));
                    NPC(nameof(ToolTip));
                }
            }
        }
    }
}