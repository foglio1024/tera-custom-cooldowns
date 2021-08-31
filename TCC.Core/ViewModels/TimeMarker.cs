using System;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;

namespace TCC.ViewModels
{
    public class TimeMarker : ThreadSafePropertyChanged
    {
        private readonly DispatcherTimer _t = new();
        private DateTime _dateTime;
        private readonly int _hourOffset;
        public string TimeString => _dateTime.ToShortTimeString();

        public double TimeFactor => (_dateTime.Hour * 60 + _dateTime.Minute) * 60 / GameEventManager.SecondsInDay;
        public string Name { get; }
        public string Color { get; }
        public TimeMarker(int hourOffset, string name, string color = "ffffff")
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
            Name = name;
            Color = color;
            _hourOffset = hourOffset;
            _dateTime = DateTime.Now.AddHours(_hourOffset);
            _t.Interval = TimeSpan.FromSeconds(1);
            _t.Tick += T_Tick;
            _t.Start();
        }

        private void T_Tick(object? sender, EventArgs e)
        {
            _dateTime = DateTime.Now.AddHours(_hourOffset);
            N(nameof(TimeString));
            N(nameof(TimeFactor));
        }
    }
}