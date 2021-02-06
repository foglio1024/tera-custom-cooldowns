using System;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Data
{
    public class StatTracker : TSPropertyChanged
    {
        private int _max = 1;
        private int _val;
        private bool _status;

        public event Action<uint>? ToZero;
        public int Val
        {
            get => _val;
            set
            {
                if (_val == value) return;
                _val = value;

                N();
                N(nameof(Factor));
                N(nameof(Maxed));
            }
        }
        public int Max
        {
            get => _max;
            set
            {
                if (_max == value) return;
                _max = value;
                if (_max == 0) _max = 1;
                N();
                N(nameof(Factor));
            }
        }
        public bool Maxed => Factor == 1;
        public double Factor => (double)_val / _max;
        public bool Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                N();
            }
        }

        public StatTracker()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void InvokeToZero(uint pDuration)
        {
            ToZero?.Invoke(pDuration);
        }
    }
}
