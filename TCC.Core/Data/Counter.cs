﻿using Nostrum.WPF.ThreadSafe;
using System;
using System.Windows.Threading;
using TeraDataLite;

namespace TCC.Data;

public class Counter : ThreadSafeObservableObject
{
    //TODO use events here
    private int _val;
    private bool _isMaxed;
    private readonly DispatcherTimer _expire;

    public int Val
    {
        get => _val;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _val)) return;
            IsMaxed = Val == MaxValue;
            RefreshTimer();
        }
    }
    public bool IsMaxed
    {
        get => _isMaxed;
        private set => RaiseAndSetIfChanged(value, ref _isMaxed);
    }
    public int MaxValue { get; private set; }
    private bool AutoExpire { get; set; }

    public Counter(int max, bool autoexpire)
    {
        Dispatcher = Dispatcher.CurrentDispatcher;
        MaxValue = max;
        AutoExpire = autoexpire;
        _expire = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(9000) }; //todo: set time from outside
        _expire.Tick += OnExpireTick;
    }

    private void OnExpireTick(object? s, EventArgs ev)
    {
        Val = 0;
    }

    private void RefreshTimer()
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