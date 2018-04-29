using System;
using System.Linq;
using System.Timers;
using System.Windows.Input;

namespace TCC.Data
{
    public class Listing : TSPropertyChanged
    {
        private uint playerId;
        private bool isRaid;
        private string message;
        private string leaderName;
        private bool isExpanded;
        private int playerCount;
        private SynchronizedObservableCollection<User> players;
        private bool _canApply = true;

        public uint LeaderId
        {
            get => Players.ToSyncArray().Count() == 0 ? playerId : Players.ToSyncArray().FirstOrDefault(x => x.IsLeader).PlayerId;
            set
            {
                if (playerId == value) return;
                playerId = value;
                NPC();
            }
        }
        public bool IsRaid
        {
            get => isRaid;
            set
            {
                if (isRaid == value) return;
                isRaid = value;
                NPC();
                NPC(nameof(MaxCount));
            }
        }
        public int PlayerCount
        {
            get
            {
                return playerCount;
            }
            set
            {
                if (playerCount == value) return;
                playerCount = value;
                NPC();
            }
        }
        public string Message
        {
            get => message;
            set
            {
                if (message == value) return;
                message = value;
                NPC();
            }
        }
        public string LeaderName
        {
            get => Players.ToSyncArray().Count() == 0 ? leaderName : Players.ToSyncArray().FirstOrDefault(x => x.IsLeader).Name;
            set
            {
                if (leaderName == value) return;
                leaderName = value;
                NPC();
            }
        }
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded == value) return;
                isExpanded = value;
                NPC();
            }
        }
        public SynchronizedObservableCollection<User> Players
        {
            get => players;
            set
            {
                if (players == value) return;
                players = value;
                NPC();
            }
        }
        public int MaxCount => IsRaid ? 30 : 5;
        public ApplyCommand Apply { get; }
        public bool CanApply
        {
            get => _canApply;
            set
            {
                if (_canApply == value) return; ;
                _canApply = value;
                NPC();
            }
        }
        public Listing()
        {
            _dispatcher = WindowManager.LfgListWindow.Dispatcher;
            Players = new SynchronizedObservableCollection<User>(_dispatcher);
            Apply = new ApplyCommand(this);
        }
    }

    public class ApplyCommand : ICommand
    {
        private Listing listing;
        private Timer t;
        public ApplyCommand(Listing listing)
        {
            this.listing = listing;
            t = new Timer() { Interval = 5000 };
            t.Elapsed += (s, ev) =>
            {
                t.Stop();
                listing.CanApply = true;
            };
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return listing.CanApply;
        }

        public void Execute(object parameter)
        {
            Proxy.ApplyToLfg(listing.LeaderId);
            listing.CanApply = false;
            t.Start();
        }
    }

}
