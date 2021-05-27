using Nostrum;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.UI;
using TeraDataLite;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.Data
{
    public class Listing : TSPropertyChanged
    {
        private uint _playerId;
        private bool _isRaid;
        private string _message = "";
        private string _leaderName = "";
        private bool _isExpanded;
        private bool _isPopupOpen;
        private int _playerCount;
        private bool _canApply = true;
        private bool _isMyLfg;
        private bool _temp;
        private readonly DateTime _createdOn;
        private bool _isFullOffline;

        public ICommand ExpandCollapseCommand { get; }
        public ICommand PostCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand BrowseTwitchCommand { get; }
        public ICommand OpenPopupCommand { get; }
        public ICommand WhisperLeaderCommand { get; }
        public ICommand ToggleAutoPublicizeCommand { get; }

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
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                if (_isPopupOpen == value) return;
                _isPopupOpen = value;
                FocusManager.PauseTopmost = _isPopupOpen;
                N();
            }
        }
        public bool Temp
        {
            get => _temp;
            set
            {
                if (_temp == value) return;
                _temp = value;
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

        public double AliveSinceMs => (DateTime.Now - _createdOn).TotalMilliseconds;


        public TSObservableCollection<User> Players { get; set; }
        public TSObservableCollection<User> Applicants { get; set; }

        public int MaxCount => IsRaid ? 30 : 5;
        public ApplyCommand ApplyCommand { get; }
        public ICommand RefreshApplicantsCommand { get; }
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

        public bool IsFullOffline
        {
            get => _isFullOffline;
            set
            {
                if(_isFullOffline == value) return;
                _isFullOffline = value;
                N();
            }
        }

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
            ApplyCommand = new ApplyCommand(this);
            RefreshApplicantsCommand = new RelayCommand(_ => StubInterface.Instance.StubClient.RequestListingCandidates(), _ => IsMyLfg);
            ExpandCollapseCommand = new RelayCommand(force =>
            {
                if (IsPopupOpen) return;
                if (force != null)
                {
                    bool bForce;
                    if (force is string s) bForce = bool.TryParse(s, out var v) && v;
                    else bForce = (bool)force;
                    if (bForce)
                    {
                        IsExpanded = !IsExpanded;
                        if (!IsExpanded) return;
                        WindowManager.ViewModels.LfgVM.LastClicked = this;
                        StubInterface.Instance.StubClient.RequestPartyInfo(LeaderId);
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
                        StubInterface.Instance.StubClient.RequestPartyInfo(LeaderId);
                    }
                }
            });
            BrowseTwitchCommand = new RelayCommand(_ =>
            {
                if (!IsTwitch) return;
                Utils.Utilities.OpenUrl(TwitchLink);
            });
            PostCommand = new RelayCommand(_ =>
            {
                var msg = Message;
                var isRaid = IsRaid;

                if (Temp) WindowManager.ViewModels.LfgVM.Listings.Remove(this);

                StubInterface.Instance.StubClient.RegisterListing(msg, isRaid);

                Task.Delay(200).ContinueWith(_ => StubInterface.Instance.StubClient.RequestListings(App.Settings.LfgWindowSettings.MinLevel, App.Settings.LfgWindowSettings.MaxLevel));

            },
            _ => Temp && !string.IsNullOrEmpty(Message));
            RemoveCommand = new RelayCommand(_ =>
            {
                if (Temp)
                    WindowManager.ViewModels.LfgVM.Listings.Remove(this);
                else
                    WindowManager.ViewModels.LfgVM.RemoveMessageCommand.Execute(null);
            });

            OpenPopupCommand = new RelayCommand(_ => IsPopupOpen = true);

            WhisperLeaderCommand = new RelayCommand(_ =>
            {
                if (!Game.InGameChatOpen) FocusManager.SendNewLine();
                FocusManager.SendString($"/w {LeaderName} ");
            });

            ToggleAutoPublicizeCommand = new RelayCommand(_ => WindowManager.ViewModels.LfgVM.ToggleAutoPublicizeCommand.Execute(null));

            _createdOn = DateTime.Now;
        }

        public Listing(ListingData l) : this()
        {
            LeaderName = l.LeaderName;
            LeaderId = l.LeaderId;
            IsRaid = l.IsRaid;
            Message = l.Message;
            PlayerCount = l.PlayerCount;
            Temp = l.Temp;
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
#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object? parameter)
        {
            return _listing.CanApply;
        }

        public async void Execute(object? parameter)
        {
            var success = await StubInterface.Instance.StubClient.ApplyToGroup(_listing.LeaderId); //ProxyOld.ApplyToLfg(_listing.LeaderId);
            if (!success) return;
            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{_listing.LeaderName}", "SMT_PARTYBOARD_APPLY");
            _listing.CanApply = false;
            _t.Start();
        }
    }
}
