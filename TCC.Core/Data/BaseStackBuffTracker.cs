using System;
using System.Windows.Threading;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class BaseStackBuffTracker : ThreadSafeObservableObject
{
    public event Action? BuffEnded;
    public event Action<int>? BaseStacksChanged;
    public event Action<long>? BaseBuffStarted;
    public event Action<long>? BaseBuffRefreshed;
    public event Action<long>? EmpoweredBuffStarted;

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static bool IsEmpoweredBuffRunning { get; set; }

    int _stacks;
    public int Stacks
    {
        get => _stacks;
        private set
        {
            if (_stacks == value) return;
            _stacks = value;
            N();
            BaseStacksChanged?.Invoke(Stacks);
        }
    }

    protected BaseStackBuffTracker()
    {
        Dispatcher = Dispatcher.CurrentDispatcher;
    }

    public virtual void StartBaseBuff(long duration)
    {
        Stacks = 1;
        BaseBuffStarted?.Invoke(duration);
    }
    public virtual void RefreshBaseBuff(int stacks, long duration)
    {
        Stacks = stacks;
        BaseBuffRefreshed?.Invoke(duration);
    }

    protected virtual void StartEmpoweredBuff(long duration)
    {
        EmpoweredBuffStarted?.Invoke(duration);
        IsEmpoweredBuffRunning = true;
        Stacks = 10;
    }

    public virtual void Stop()
    {
        IsEmpoweredBuffRunning = false;
        Stacks = 0;
        BuffEnded?.Invoke();
    }
}