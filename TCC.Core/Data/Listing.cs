using System;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using TCC.Data.Pc;
using TCC.ViewModels;

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
        private SynchronizedObservableCollection<User> _applicants;
        private bool _canApply = true;

        public uint LeaderId
        {
            get
            {
                return Players.ToSyncArray().Length == 0
                    ? _playerId
                    // ReSharper disable once PossibleNullReferenceException
                    : Players.ToSyncArray().FirstOrDefault(x => x.IsLeader)?.PlayerId ?? 0;
            }
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
                _message = value.Replace("&gt;", ">").Replace("&lt;", "<");
                NPC();
                NPC(nameof(IsTrade));
                NPC(nameof(IsTwitch));
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

        public bool IsMyLfg => Dispatcher.Invoke(()=> Players.Any(x => x.PlayerId == SessionManager.CurrentPlayer.PlayerId) || 
                               LeaderId == SessionManager.CurrentPlayer.PlayerId ||
                                GroupWindowViewModel.Instance.Members.ToSyncArray().Any(member => member.PlayerId == LeaderId));
        public bool IsTrade => _message.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               _message.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               _message.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) != -1;
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
        public SynchronizedObservableCollection<User> Applicants
        {
            get => _applicants;
            set
            {
                if (_applicants == value) return;
                _applicants= value;
                NPC();
            }
        }
        public int MaxCount => IsRaid ? 30 : 5;
        public ApplyCommand Apply { get; }
        public RefreshApplicantsCommand RefreshApplicants { get; }
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

        public string TwitchLink
        {
            get
            {
                var username = "";
                var split = _message.Split(' ').ToList();
                var twLink = split.FirstOrDefault(x =>
                    x.IndexOf("twitch.tv", StringComparison.InvariantCultureIgnoreCase) != -1);
                var splitLink = twLink?.Split('/');
                if (splitLink != null && splitLink.Length >= 2) username = splitLink[1]; 
                return $"https://www.twitch.tv/{username}";
            }
        }

        public bool IsTwitch => _message.Contains("twitch.tv");

        public void NotifyMyLfg()
        {
            NPC(nameof(IsMyLfg));
        }

        public Listing()
        {
            Dispatcher = App.BaseDispatcher; //TODO check this
            Players = new SynchronizedObservableCollection<User>(Dispatcher);
            Applicants = new SynchronizedObservableCollection<User>(Dispatcher);
            Apply = new ApplyCommand(this);
            RefreshApplicants = new RefreshApplicantsCommand(this);
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
            Proxy.Proxy.ApplyToLfg(_listing.LeaderId);
            _listing.CanApply = false;
            _t.Start();
        }
    }
    public class RefreshApplicantsCommand : ICommand
    {
        private readonly Listing _listing;
        public RefreshApplicantsCommand(Listing listing)
        {
            _listing = listing;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _listing.IsMyLfg;
        }

        public void Execute(object parameter)
        {
            Proxy.Proxy.RequestCandidates();
        }
    }

}
