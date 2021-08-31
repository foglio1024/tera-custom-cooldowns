using System;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TeraDataLite;

namespace TCC.Data
{
    public class Counter : ThreadSafePropertyChanged
    {
        //TODO use events here
        private int _val;
        private bool _isMaxed;
        private readonly DispatcherTimer _expire;

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
        public int MaxValue { get; set; }
        public bool AutoExpire { get; set; }


        public Counter(int max, bool autoexpire)
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
            MaxValue = max;
            AutoExpire = autoexpire;
            _expire = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(9000) };
            _expire.Tick += OnExpireTick;
        }

        private void OnExpireTick(object? s, EventArgs ev)
        {
            Val = 0;
        }

        private void RefreshTimer()
        {
            _expire.Stop();
            if (_val == 0) return;
            if (!AutoExpire) return;
            _expire.Start();
        }

        public void SetClass(Class c)
        {
            switch (c)
            {
                case Class.Warrior: 
                    MaxValue = 10;
                    AutoExpire = true;
                    break;
                case Class.Valkyrie:
                    MaxValue = 7;
                    AutoExpire = false;
                    break;
            }
        }
    }
}
