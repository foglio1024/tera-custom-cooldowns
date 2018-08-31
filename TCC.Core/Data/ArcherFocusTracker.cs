using System;
using System.Windows.Threading;

namespace TCC.Data
{
    public class ArcherFocusTracker : TSPropertyChanged
    {
        public event Action<int> StacksChanged;
        public event Action<long> FocusStarted;
        public event Action<long> Refreshed;
        public event Action<long> FocusXStarted;
        public event Action FocusEnded;

        public string Icon { get; }
        public readonly uint Duration = 10000;
        private int _stacks;
        private static bool _isFocusXRunning;

        public int Stacks
        {
            get => _stacks;
            private set
            {
                if (_stacks == value) return;
                _stacks = value;
                NPC(nameof(Stacks));
                StacksChanged?.Invoke(Stacks);
            }
        }

        public static bool IsFocusXRunning
        {
            get { return _isFocusXRunning; }
            set { _isFocusXRunning = value; }
        }

        public ArcherFocusTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            if (SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(601400, out var ab))
            {
                Icon = ab.IconName;
            }
        }

        public void StartFocus(long duration)
        {
            Stacks = 1;
            FocusStarted?.Invoke(duration);
        }
        public void SetFocusStacks(int stacks, long duration)
        {
            Stacks = stacks;
            Refreshed?.Invoke(duration);
        }
        public void StartFocusX(long duration)
        {
            FocusXStarted?.Invoke(duration);
            IsFocusXRunning = true;
            Stacks = 10;
        }
        public void StopFocusX()
        {
            IsFocusXRunning = false;
            Stacks = 0;
            FocusEnded?.Invoke();
        }

        public void StopFocus()
        {
            IsFocusXRunning = false;
            Stacks = 0;
            FocusEnded?.Invoke();
        }
    }
}
