using System;
using System.Linq;
using System.Timers;
using System.Windows.Input;

namespace TCC.Data
{
    public class Listing : TSPropertyChanged
    {
        private uint _playerId;
        private bool _isRaid;
        private string _message;
        private string _leaderName;
        private bool _isExpanded;
        private int _playerCount;
        private SynchronizedObservableCollection<User> _players;
        private bool _canApply = true;

        public uint LeaderId
        {
            get => Players.ToSyncArray().Length == 0
                ? _playerId
                // ReSharper disable once PossibleNullReferenceException
                : Players.ToSyncArray().FirstOrDefault(x => x.IsLeader).PlayerId;
            set
            {
                if (_playerId == value) return;
                _playerId = value;
                NPC();
            }
        }
        public bool IsRaid
        {
            get => _isRaid;
            set
            {
                if (_isRaid == value) return;
                _isRaid = value;
                NPC();
                NPC(nameof(MaxCount));
            }
        }
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                if (_playerCount == value) return;
                _playerCount = value;
                NPC();
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                _message = value;
                NPC();
            }
        }
        public string LeaderName
        {
            get => Players.ToSyncArray().Length == 0 ? _leaderName : Players.ToSyncArray().FirstOrDefault(x => x.IsLeader)?.Name;
            set
            {
                if (_leaderName == value) return;
                _leaderName = value;
                NPC();
            }
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                NPC();
            }
        }
        public SynchronizedObservableCollection<User> Players
        {
            get => _players;
            set
            {
                if (_players == value) return;
                _players = value;
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
                if (_canApply == value) return;
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
        private readonly Listing _listing;
        private readonly Timer _t;
        public ApplyCommand(Listing listing)
        {
            _listing = listing;
            _t = new Timer() { Interval = 5000 };
            _t.Elapsed += (s, ev) =>
            {
                _t.Stop();
                listing.CanApply = true;
            };
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _listing.CanApply;
        }

        public void Execute(object parameter)
        {
            Proxy.ApplyToLfg(_listing.LeaderId);
            _listing.CanApply = false;
            _t.Start();
        }
    }

}
