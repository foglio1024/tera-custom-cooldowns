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
                NPC("Stacks");
            }
        }

        public ArcherFocusTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            if (SessionManager.AbnormalityDatabase.Abnormalities.TryGetValue(601400, out var ab))
            {
                Icon = ab.IconName;
            }
        }

        public void StartFocus()
        {
            Stacks = 1;
            NPC("StartFocus");
        }
        public void SetFocusStacks(int stacks)
        {
            Stacks = stacks;
            NPC("Refresh");
        }
        public void StartFocusX()
        {
            Stacks = 10;
            NPC("StartFocusX");
        }
        public void StopFocus()
        {
            //if (Stacks >= 9) return;
            Stacks = 0;
            NPC("Ended");
        }
    }
}
