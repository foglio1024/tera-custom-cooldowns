using System;
using System.Collections.Generic;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class StanceTracker<T> : ThreadSafeObservableObject where T : struct, IComparable
{
    T _currentStance;

    public T CurrentStance
    {
        get => _currentStance;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_currentStance, value)) return;
            _currentStance = value;
            N();
        }
    }
}
