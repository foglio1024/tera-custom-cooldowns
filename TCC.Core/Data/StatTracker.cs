using System;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class StatTracker : ThreadSafeObservableObject
{
    public event Action<uint>? ToZero;
    public event Action<double>? FactorChanged;

    int _max = 1;
    int _val;
    bool _status;
    bool _maxed;
    double _factor;

    public int Val
    {
        get => _val;
        set
        {
            if (_val == value) return;
            _val = value;
            N();

            Factor = (double)_val / _max;
            Maxed = Factor == 1;
        }
    }
    public int Max
    {
        get => _max;
        set
        {
            if (_max == value) return;
            _max = value;
            if (_max == 0) _max = 1;
            N();

            Factor = (double)_val / _max;
            Maxed = Factor == 1;
        }
    }
    public bool Maxed
    {
        get { return _maxed; }
        private set
        {
            if (_maxed == value) return;
            _maxed = value;
            N();
        }
    }
    public double Factor
    {
        get { return _factor; }
        private set
        {
            if (_factor == value) return;
            _factor = value;
            Dispatcher.Invoke(InvokeFactorChanged);
            N();
        }
    }
    public bool Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            N();
        }
    }

    public void InvokeToZero(uint pDuration)
    {
        ToZero?.Invoke(pDuration);
    }

    void InvokeFactorChanged()
    {
        FactorChanged?.Invoke(_factor);
    }
}