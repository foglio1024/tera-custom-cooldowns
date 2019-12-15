using FoglioUtils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.ViewModels;
using TeraDataLite;

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
        private TSObservableCollection<User> _players;
        private TSObservableCollection<User> _applicants;
        private bool _canApply = true;
        private bool _isMyLfg;

        public ICommand ExpandCollapseCommand { get; }
        public ICommand BrowseTwitchCommand { get; }

        public uint LeaderId
        {
            get
            {
                return _playerId;
                //return Players.ToSyncList().Count == 0
                //    ? _playerId
                //    : Players.ToSyncList().FirstOrDefault(x => x.IsLeader)?.PlayerId ?? 0;
            }
            set
            {
                if (_playerId == value) return;
                _playerId = value;
                N();
            }
        }

        public bool IsRaid
        {
            get => _isRaid;
            set
            {
                if (_isRaid == value) return;
                _isRaid = value;
                N();
                N(nameof(MaxCount));
            }
        }
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                if (_playerCount == value) return;
                _playerCount = value;
                N();
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;
                _message = value.Replace("&gt;", ">").Replace("&lt;", "<");
                N();
                N(nameof(IsTrade));
                N(nameof(IsTwitch));
            }
        }
        public string LeaderName
        {
            get => _leaderName; //Players.ToSyncList().Count == 0 ? _leaderName : Players.ToSyncList().FirstOrDefault(x => x.IsLeader)?.Name;
            set
            {
                if (_leaderName == value) return;
                _leaderName = value;
                N();
            }
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                N();
            }
        }

        public bool IsMyLfg
        {
            get => _isMyLfg;
            set
            {
                if (_isMyLfg == value) return;
                _isMyLfg = value;
                N();
            }
        }

        public bool IsTrade => _message.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               _message.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                               _message.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) != -1;
        public TSObservableCollection<User> Players
        {
            get => _players;
            set
            {
                if (_players == value) return;
                _players = value;
                N();
            }
        }
        public TSObservableCollection<User> Applicants
        {
            get => _applicants;
            set
            {
                if (_applicants == value) return;
                _applicants = value;
                N();
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
                N();
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

        public bool IsTwitch => _message.IndexOf("twitch.tv", StringComparison.InvariantCultureIgnoreCase) != -1;

        public void UpdateIsMyLfg()
        {
            Dispatcher.InvokeAsync(() =>
            {
                IsMyLfg = Players.Any(x => x.PlayerId == Game.Me.PlayerId) ||
                          LeaderId == Game.Me.PlayerId ||
                          WindowManager.ViewModels.GroupVM.Members.ToSyncList().Any(member => member.PlayerId == LeaderId);
            }, DispatcherPriority.DataBind);
        }



        public Listing()
        {
            Dispatcher = App.BaseDispatcher;
            Players = new TSObservableCollection<User>(Dispatcher);
            Applicants = new TSObservableCollection<User>(Dispatcher);
            Apply = new ApplyCommand(this);
            RefreshApplicants = new RefreshApplicantsCommand(this);
            ExpandCollapseCommand = new RelayCommand(force =>
            {
                if (force != null)
                {
                    if ((bool)force)
                    {
                        if (IsExpanded) return;
                        //todo: expand
                    }
                    else
                    {
                        if (IsExpanded) IsExpanded = false;
                    }
                }
                else
                {
                    if (IsExpanded)
                    {
                        IsExpanded = false;
                    }
                    else
                    {
                        WindowManager.ViewModels.LfgVM.LastClicked = this;
                        ProxyInterface.Instance.Stub.RequestPartyInfo(LeaderId);
                    }
                }
            });
            BrowseTwitchCommand = new RelayCommand(_ =>
            {
                if (!IsTwitch) return;
                Process.Start(TwitchLink);
            });

        }

        public Listing(ListingData l) : this()
        {
            LeaderName = l.LeaderName;
            LeaderId = l.LeaderId;
            IsRaid = l.IsRaid;
            Message = l.Message;
            PlayerCount = l.PlayerCount;
        }
    }

    public class ApplyCommand : ICommand
    {
        private readonly Listing _listing;
        private readonly Timer _t;
        public ApplyCommand(Listing listing)
        {
            _listing = listing;
            _t = new Timer { Interval = 5000 };
            _t.Elapsed += OnTimerElapsed;
        }

        void OnTimerElapsed(object s, ElapsedEventArgs ev)
        {
            _t.Stop();
            _listing.CanApply = true;
        }
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        public bool CanExecute(object parameter)
        {
            return _listing.CanApply;
        }

        public async void Execute(object parameter)
        {
            var success = await ProxyInterface.Instance.Stub.ApplyToGroup(_listing.LeaderId); //ProxyOld.ApplyToLfg(_listing.LeaderId);
            if (!success) return;
            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{_listing.LeaderName}", "SMT_PARTYBOARD_APPLY");
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
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        public bool CanExecute(object parameter)
        {
            return _listing.IsMyLfg;
        }

        public void Execute(object parameter)
        {
            ProxyInterface.Instance.Stub.RequestListingCandidates(); //ProxyOld.RequestCandidates();
        }
    }

}
