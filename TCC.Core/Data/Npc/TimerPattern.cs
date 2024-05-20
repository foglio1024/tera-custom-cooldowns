using System;
using System.Timers;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Npc;

public class TimerPattern : ThreadSafeObservableObject, IDisposable
{
    public event Action? Started;
    public event Action? Ended;

    private readonly Timer _timer;
    protected bool IsRunning => _timer.Enabled;
    protected Npc? Target { get; set; }
    public int Duration { get; }

    protected TimerPattern(int duration)
    {
        Duration = duration;
        _timer = new Timer(Duration * 1000);
        _timer.Elapsed += OnTimerElapsed;
    }

    protected void Start()
    {
        _timer.Start();
        Started?.Invoke();
    }

    protected void Reset()
    {
        _timer.Stop();
        Ended?.Invoke();
    }

    public virtual void SetTarget(Npc target)
    {
        Target = target;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        Reset();
    }

    public void Dispose()
    {
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Stop();
        _timer.Dispose();
    }
}