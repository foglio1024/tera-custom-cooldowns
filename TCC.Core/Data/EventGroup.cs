using System;
using System.Globalization;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.UI;
using TCC.ViewModels;

namespace TCC.Data
{
    public class EventGroup : ThreadSafePropertyChanged
    {
        public ThreadSafeObservableCollection<DailyEvent> Events { get; }
        public string Name { get; }
        public bool RemoteCheck { get; }
        private DateTime _start;
        private DateTime _end;
        public string TimeSpanTooltip
        {
            get
            {
                if(_start == DateTime.MinValue && _end == DateTime.MaxValue) return $"{Name} (permanent)";
                var startTime = _start.Hour == 0 &&_start.Minute == 0? _start.ToShortDateString() : _start.ToString(CultureInfo.InvariantCulture);
                var endTime = _end.Hour == 0 && _end.Minute == 0 ? _end.ToShortDateString() : $"{_end}";
                return $"{Name}: from {startTime} to {endTime}";
            }
        }

        public EventGroup(string name, DateTime start, DateTime end, bool rc)
        {
            SetDispatcher(WindowManager.DashboardWindow.Dispatcher);
            Events = new ThreadSafeObservableCollection<DailyEvent>(_dispatcher);
            Name = name;
            RemoteCheck = rc;
            _start = start;
            _end = end;
        }
        public void AddEvent(DailyEvent ev)
        {
            _dispatcher.Invoke(() => Events.Add(ev));
            //Events.Insert(0, ev);
        }
    }
}