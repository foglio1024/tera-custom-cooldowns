using System;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data
{
    public class BaseStackBuffTracker : ThreadSafePropertyChanged
    {
        public event Action? BuffEnded;
        public event Action<int>? BaseStacksChanged;
        public event Action<long>? BaseBuffStarted;
        public event Action<long>? BaseBuffRefreshed;
        public event Action<long>? EmpoweredBuffStarted;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static bool IsEmpoweredBuffRunning { get; set; }

        private int _stacks;
        public int Stacks
        {
            get => _stacks;
            protected set
            {
                if (_stacks == value) return;
                _stacks = value;
                N(nameof(Stacks));
                BaseStacksChanged?.Invoke(Stacks);
            }
        }

        protected BaseStackBuffTracker()
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
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
        public virtual void StartEmpoweredBuff(long duration)
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
}