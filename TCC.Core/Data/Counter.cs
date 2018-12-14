using System;
using System.Windows.Threading;

namespace TCC.Data
{
    public class Counter : TSPropertyChanged
    {
        //TODO use events here
        private int _val;
        private bool _isMaxed;
        private readonly DispatcherTimer _expire;
        private readonly bool _autoexpire;

        public int Val
        {
            get => _val;
            set
            {
                if (_val == value) return;
                _val = value;
                IsMaxed = Val == MaxValue;
                RefreshTimer();
                N();
            }
        }

        public bool IsMaxed
        {
            get => _isMaxed;
            private set
            {
                if (_isMaxed == value) return;
                _isMaxed = value;
                N();
            }
        }

        public int MaxValue { get; }
        public Counter(int max, bool autoexpire)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            MaxValue = max;
            _autoexpire = autoexpire;
            _expire = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(9000) };
            _expire.Tick += (s, ev) => Val = 0;
        }
        private void RefreshTimer()
        {
            _expire.Stop();
            if (_val == 0) return;
            if (!_autoexpire) return;
            _expire.Start();
        }
    }
}
