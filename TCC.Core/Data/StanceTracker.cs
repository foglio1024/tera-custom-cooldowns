using Nostrum.WPF.ThreadSafe;
using System;

namespace TCC.Data;

public class StanceTracker<T> : ThreadSafeObservableObject where T : struct, IComparable
{
    private T _currentStance;

    public T CurrentStance
    {
        get => _currentStance;
        set => RaiseAndSetIfChanged(value, ref _currentStance);
    }
}
