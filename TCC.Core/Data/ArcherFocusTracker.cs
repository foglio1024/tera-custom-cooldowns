using System.Windows.Threading;
namespace TCC.Data
{
    public class ArcherFocusTracker : TSPropertyChanged
    {
        public string Icon { get; private set; }
        public readonly uint Duration = 10000;
        private int stacks;
        public int Stacks
        {
            get => stacks;
            private set
            {
                if (stacks == value) return;
                stacks = value;
                NotifyPropertyChanged("Stacks");
            }
        }

        public ArcherFocusTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            if (AbnormalityDatabase.Abnormalities.TryGetValue(601400, out Abnormality ab))
            {
                Icon = ab.IconName;
            }
        }

        public void StartFocus()
        {
            Stacks = 1;
            NotifyPropertyChanged("StartFocus");
        }
        public void SetFocusStacks(int stacks)
        {
            Stacks = stacks;
            NotifyPropertyChanged("Refresh");
        }
        public void StartFocusX()
        {
            Stacks = 10;
            NotifyPropertyChanged("StartFocusX");
        }
        public void StopFocus()
        {
            //if (Stacks >= 9) return;
            Stacks = 0;
            NotifyPropertyChanged("Ended");
        }
    }
}
