using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class IntTracker : TSPropertyChanged
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
            }
        }
        public IntTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
