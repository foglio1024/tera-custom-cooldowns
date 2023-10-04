using System;
using Nostrum.WPF.ThreadSafe;

namespace TCC.ViewModels;

public class DailyEvent : ThreadSafeObservableObject
{
    DateTime Start { get; set; }
    TimeSpan Duration { get; set; }
    readonly TimeSpan _realDuration;
    public double StartFactor => 60 * (Start.Hour * 60 + Start.Minute) / GameEventManager.SecondsInDay;
    public double DurationFactor => Duration.TotalSeconds / GameEventManager.SecondsInDay;
    bool _happened;
    public bool IsClose
    {
        get
        {
            var ts = Start - GameEventManager.Instance.CurrentServerTime;
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

    public async void UpdateFromServer(bool force = false)
    {
        if (GameEventManager.Instance.CurrentServerTime < Start || _happened) return;
        var time = force
            ? GameEventManager.Instance.CurrentServerTime
            : (await GameEventManager.Instance.RetrieveGuildBamDateTime()).AddHours(GameEventManager.Instance.ServerHourOffsetFromUtc);

        if (time < Start) return;
        Start = time;
        Duration = TimeSpan.Zero;
        _happened = true;
        N(nameof(StartFactor));
        N(nameof(DurationFactor));
        N(nameof(ToolTip));
    }
}