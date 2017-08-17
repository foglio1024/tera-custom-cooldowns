using System;
using System.Security.Cryptography;
using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class EventGroup : TSPropertyChanged
    {
        public SynchronizedObservableCollection<DailyEvent> Events { get; }
        public string Name { get; }
        public bool RemoteCheck { get; }
        private DateTime _start;
        private DateTime _end;
        public string TimeSpanTooltip
        {
            get
            {
                if(_start == DateTime.MinValue && _end == DateTime.MaxValue) return $"{Name} (permanent)";
                var startTime = _start.Hour == 0 &&_start.Minute == 0? _start.ToShortDateString() : _start.ToString();
                var endTime = _end.Hour == 0 && _end.Minute == 0 ? _end.ToShortDateString() : $"{_end}";
                return $"{Name}: from {startTime} to {endTime}";
            }
        }

        public EventGroup(string name, DateTime start, DateTime end, bool rc)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Events = new SynchronizedObservableCollection<DailyEvent>(_dispatcher);
            Name = name;
            RemoteCheck = rc;
            _start = start;
            _end = end;
        }
        public void AddEvent(DailyEvent ev)
        {
            Events.Insert(0, ev);
        }
    }
}