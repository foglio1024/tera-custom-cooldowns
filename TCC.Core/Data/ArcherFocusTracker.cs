using System;
using System.Windows.Threading;
using TCC.ClassSpecific;

namespace TCC.Data
{
    public class BaseStackBuffTracker : TSPropertyChanged
    {
        public event Action BuffEnded;
        public event Action<int> BaseStacksChanged;
        public event Action<long> BaseBuffStarted;
        public event Action<long> BaseBuffRefreshed;
        public event Action<long> EmpoweredBuffStarted;

        public static bool IsEmpoweredBuffRunning { get; set; }

        public string Icon { get; protected set; }

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

        public BaseStackBuffTracker()
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

    public class ArcherFocusTracker : BaseStackBuffTracker
    {
        public ArcherFocusTracker() : base()
        {
            if (SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities.TryGetValue(601400, out var ab))
            {
                Icon = ab.IconName;
            }
        }
        public void StartFocus(long duration)
        {
            base.StartBaseBuff(duration);
        }
        public void SetFocusStacks(int stacks, long duration)
        {
            base.RefreshBaseBuff(stacks, duration);
        }
        public void StartFocusX(long duration)
        {
            base.StartEmpoweredBuff(duration);
        }
        public void StopFocusX()
        {
            base.Stop();
        }
        public void StopFocus()
        {
            base.Stop();
        }
    }

    public class LancerLineHeldTracker : BaseStackBuffTracker
    {
        public LancerLineHeldTracker()
        {
            if (!SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities.TryGetValue(LancerAbnormalityTracker.LineHeldId, out var ab)) return;
            Icon = ab.IconName;
            BaseStacksChanged += (stacks) => { if (stacks == 0) Stop(); };
        }
    }
}
