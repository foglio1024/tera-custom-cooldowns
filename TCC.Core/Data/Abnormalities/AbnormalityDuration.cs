using Nostrum.WPF.ThreadSafe;
using System;
using System.Timers;
using System.Windows.Threading;
using TCC.Debugging;

namespace TCC.Data.Abnormalities;

public class AbnormalityDuration : ThreadSafeObservableObject, IDisposable
{
    public event Action? Refreshed;

    public ulong Target { get; }
    public Abnormality Abnormality { get; }

    readonly Timer _timer;
    uint _duration;
    int _stacks;
    double _durationLeft;
    bool _isTimerDisposed;
    DateTime _startTime;
    DateTime _endTime;

    public uint Duration
    {
        get => _duration;
        set
        {
            if (value == _duration) return;
            _duration = value;
            N();
        }
    }
    public int Stacks
    {
        get => _stacks;
        set
        {
            if (value == _stacks) return;
            _stacks = value;
            N();
        }
    }
    public double DurationLeft
    {
        get => _durationLeft;
        set
        {
            if (value == _durationLeft) return;
            _durationLeft = value;
            N();
        }
    }
    public bool Animated { get; private set; }

    AbnormalityDuration(Abnormality b)
    {
        Abnormality = b;
        _timer = new Timer { Interval = 900 };
        _isTimerDisposed = false;
    }
    public AbnormalityDuration(Abnormality b, uint d, int s, ulong t, Dispatcher disp, bool animated) : this(b)
    {
        ObjectTracker.Register(this.GetType());
        Dispatcher = disp;
        Animated = animated;
        Duration = d;
        Stacks = s;
        Target = t;
        DurationLeft = d;
        _startTime = DateTime.Now;
        _endTime = DateTime.Now.AddMilliseconds(d);

        if (Abnormality.Infinity) return;
        _timer.Elapsed += DecreaseDuration;
        _timer.Start();
    }

    ~AbnormalityDuration()
    {
        ObjectTracker.Unregister(this.GetType());
    }

    void DecreaseDuration(object? sender, EventArgs e)
    {
        DurationLeft = (_endTime - DateTime.Now).TotalMilliseconds;
        if (DurationLeft < 0) DurationLeft = 0;
        if (DurationLeft > 0) return;
        _timer.Stop();
    }
    public void Refresh()
    {
        if (_isTimerDisposed) return;
        _timer.Stop();
        _startTime = DateTime.Now;
        _endTime = _startTime.AddMilliseconds(Duration);
        if (Duration != 0) _timer.Start();
        Refreshed?.Invoke();
    }
    public void Dispose()
    {
        _timer.Stop();
        _timer.Elapsed -= DecreaseDuration;
        _timer.Dispose();
        _isTimerDisposed = true;
    }
}