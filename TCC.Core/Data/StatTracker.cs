using System;
using System.Windows.Threading;

namespace TCC.Data
{
    public class StatTracker : TSPropertyChanged
    {

        public event Action<uint> OnToZero;
        private int val = 0;
        public int Val
        {
            get { return val; }
            set
            {
                if (val == value) return;
                val = value;

                NPC("Val");
                NPC("Factor");
                Maxed = Val == Max;
                NPC(nameof(Maxed));
            }
        }
        public bool Maxed { get => Factor ==1; set { }
        }
        private int max = 1;
        public int Max
        {
            get => max;
            set
            {
                if (max == value) return;
                max = value;
                if (max == 0) max = 1;
                NPC("Max");
                NPC("Factor");
            }
        }

        public double Factor => (double)val / max;

        private bool status = false;
        public bool Status
        {
            get => status;
            set
            {
                if (status == value) return;
                status = value;
                NPC("Status");
            }
        }

        public StatTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void ToZero(uint pDuration)
        {
            OnToZero?.Invoke(pDuration);
        }
    }
}
