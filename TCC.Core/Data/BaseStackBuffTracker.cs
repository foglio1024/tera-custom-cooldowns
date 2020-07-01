using System;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Data
{
    public class BaseStackBuffTracker : TSPropertyChanged
    {
        public event Action BuffEnded = null!;
        public event Action<int> BaseStacksChanged = null!;
        public event Action<long> BaseBuffStarted = null!;
        public event Action<long> BaseBuffRefreshed = null!;
        public event Action<long> EmpoweredBuffStarted = null!;

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