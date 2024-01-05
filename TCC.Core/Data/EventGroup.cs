using System;
using System.Globalization;
using Nostrum.WPF.ThreadSafe;
using TCC.UI;
using TCC.ViewModels;

namespace TCC.Data;

public class EventGroup : ThreadSafeObservableObject
{
    readonly DateTime _start;
    readonly DateTime _end;

    public string Name { get; }
    public bool RemoteCheck { get; }
    public string TimeSpanTooltip { get; }
    public ThreadSafeObservableCollection<DailyEvent> Events { get; }

    public EventGroup(string name, DateTime start, DateTime end, bool rc)
    {
        Dispatcher = WindowManager.DashboardWindow.Dispatcher;
        Events = new ThreadSafeObservableCollection<DailyEvent>(_dispatcher);
        Name = name;
        RemoteCheck = rc;
        _start = start;
        _end = end;

        TimeSpanTooltip = BuildTooltip();

        return;

        string BuildTooltip()
        {
            if (_start == DateTime.MinValue && _end == DateTime.MaxValue)
            {
                return $"{Name} (permanent)";
            }

            var startTime = _start is { Hour: 0, Minute: 0 }
                ? _start.ToShortDateString()
                : _start.ToString(CultureInfo.InvariantCulture);
            var endTime = _end is { Hour: 0, Minute: 0 }
                ? _end.ToShortDateString()
                : $"{_end}";

            return $"{Name}: from {startTime} to {endTime}";
        }
    }

    public void AddEvent(DailyEvent ev)
    {
        _dispatcher.Invoke(() => Events.Add(ev));
    }
}