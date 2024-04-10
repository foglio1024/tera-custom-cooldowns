using System;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class StatTracker : ThreadSafeObservableObject
{
    public event Action<uint>? ToZero;
    public event Action<double>? FactorChanged;

    private int _max = 1;
    private int _val;
    private bool _status;
    private bool _maxed;
    private double _factor;

    public int Val
    {
        get => _val;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _val)) return;

            Factor = (double)_val / _max;
            Maxed = Factor == 1;
        }
    }
    public int Max
    {
        get => _max;
        set
        {
            if (!RaiseAndSetIfChanged(value != 0 ? value : 1, ref _max)) return;

            Factor = (double)_val / _max;
            Maxed = Factor == 1;
        }
    }
    public bool Maxed
    {
        get { return _maxed; }
        private set => RaiseAndSetIfChanged(value, ref _maxed);
    }
    public double Factor
    {
        get { return _factor; }
        private set
        {
            if (!RaiseAndSetIfChanged(value, ref _factor)) return;
            Dispatcher.Invoke(InvokeFactorChanged);
        }
    }
    public bool Status
    {
        get => _status;
        set => RaiseAndSetIfChanged(value, ref _status);
    }

    public void InvokeToZero(uint pDuration)
    {
        ToZero?.Invoke(pDuration);
    }

    private void InvokeFactorChanged()
    {
        FactorChanged?.Invoke(_factor);
    }
}