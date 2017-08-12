using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class StatTracker : TSPropertyChanged
    {
        private int val = 0;
        public int Val
        {
            get { return val; }
            set
            {
                if (val == value) return;
                val = value;

                NotifyPropertyChanged("Val");
                NotifyPropertyChanged("Factor");
                Maxed = Val == Max;
                NotifyPropertyChanged(nameof(Maxed));
            }
        }
        public bool Maxed { get; set; }
        private int max = 1;
        public int Max
        {
            get => max;
            set
            {
                if (max == value) return;
                max = value;
                if (max == 0) max = 1;
                NotifyPropertyChanged("Max");
                NotifyPropertyChanged("Factor");
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
                NotifyPropertyChanged("Status");
            }
        }

        public StatTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
