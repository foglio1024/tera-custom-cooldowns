using Nostrum.WPF.ThreadSafe;
using System;
using System.Windows.Threading;
using TeraDataLite;

namespace TCC.Data;

public class Counter : ThreadSafeObservableObject
{
    //TODO use events here
    int _val;
    bool _isMaxed;
    readonly DispatcherTimer _expire;

    public int Val
    {
        get => _val;
        set
        {
            if (_val == value) return;
            _val = value;
            IsMaxed = Val == MaxValue;
            RefreshTimer();
            N();
        }
    }
    public bool IsMaxed
    {
        get => _isMaxed;
        private set
        {
            if (_isMaxed == value) return;
            _isMaxed = value;
            N();
        }
    }
    public int MaxValue { get; private set; }
    bool AutoExpire { get; set; }

    public Counter(int max, bool autoexpire)
    {
        Dispatcher = Dispatcher.CurrentDispatcher;
        MaxValue = max;
        AutoExpire = autoexpire;
        _expire = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(9000) }; //todo: set time from outside
        _expire.Tick += OnExpireTick;
    }

    void OnExpireTick(object? s, EventArgs ev)
    {
        Val = 0;
    }

    void RefreshTimer()
    {
        _expire.Stop();

        if (_val == 0) return;
        if (!AutoExpire) return;

        _expire.Start();
    }

    public void SetClass(Class c)
    {
        switch (c)
        {
            case Class.Warrior:
                MaxValue = 10;
                AutoExpire = true;
                break;
            case Class.Valkyrie:
                MaxValue = 7;
                AutoExpire = false;
                break;
        }
    }
}