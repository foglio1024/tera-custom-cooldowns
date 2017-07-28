using System;
using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class Counter : TSPropertyChanged
    {
        private int val = 0;
        public int Val
        {
            get => val;
            set
            {
                if (val == value) return;
                val = value;
                if (val == _max)
                {
                    NotifyPropertyChanged("Maxed");
                }
                RefreshTimer();
                NotifyPropertyChanged("Val");
            }
        }

        DispatcherTimer _expire;
        int _max;
        bool _autoexpire;
        void RefreshTimer()
        {
            _expire.Stop();
            if (val == 0) return;
            if (!_autoexpire) return;
            _expire.Start();
        }
        public Counter(int max, bool autoexpire)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _max = max;
            _autoexpire = autoexpire;
            _expire = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(9000) };
            _expire.Tick += (s, ev) => Val = 0;
        }
    }
}
