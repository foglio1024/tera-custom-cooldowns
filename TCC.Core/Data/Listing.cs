using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.UI;
using TeraDataLite;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.Data;

public class Listing : ThreadSafeObservableObject
{
    bool _isRaid;
    string _message = "";
    string _leaderName = "";
    bool _isExpanded;
    bool _isPopupOpen;
    int _playerCount;
    bool _canApply = true;
    bool _isMyLfg;
    readonly bool _temp;
    readonly DateTime _createdOn;
    bool _isFullOffline;

    public uint LeaderId { get; set; }
    public uint ServerId { get; set; }
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
        get => _leaderName;
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
        private init
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
    public bool IsTrade => _message.Contains("WTS", StringComparison.InvariantCultureIgnoreCase) ||
                           _message.Contains("WTB", StringComparison.InvariantCultureIgnoreCase) ||
                           _message.Contains("WTT", StringComparison.InvariantCultureIgnoreCase);
    public double AliveSinceMs => (DateTime.Now - _createdOn).TotalMilliseconds;
    public int MaxCount => IsRaid ? 30 : 5;
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
            var split = _message.Split(' ');
            var twLink = split.FirstOrDefault(x =>
                x.Contains("twitch.tv", StringComparison.InvariantCultureIgnoreCase));
            var splitLink = twLink?.Split('/');
            if (splitLink is { Length: >= 2 }) username = splitLink[1];
            return $"https://www.twitch.tv/{username}";
        }
    }
    public bool IsTwitch => _message.Contains("twitch.tv", StringComparison.InvariantCultureIgnoreCase);
    public bool IsFullOffline
    {
        get => _isFullOffline;
        set
        {
            if (_isFullOffline == value) return;
            _isFullOffline = value;
            N();
        }
    }
    public ThreadSafeObservableCollection<User> Players { get; }
    public ThreadSafeObservableCollection<User> Applicants { get; }

    public ICommand ExpandCollapseCommand { get; }
    public ICommand PostCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand BrowseTwitchCommand { get; }
    public ICommand OpenPopupCommand { get; }
    public ICommand WhisperLeaderCommand { get; }
    public ICommand ToggleAutoPublicizeCommand { get; }
    public ICommand RefreshApplicantsCommand { get; }
    public ApplyCommand ApplyCommand { get; }

    public Listing()
    {
        Dispatcher = App.BaseDispatcher;
        Players = new ThreadSafeObservableCollection<User>(_dispatcher);
        Applicants = new ThreadSafeObservableCollection<User>(_dispatcher);
        ApplyCommand = new ApplyCommand(this);
        RefreshApplicantsCommand = new RelayCommand(StubInterface.Instance.StubClient.RequestListingCandidates, _ => IsMyLfg);
        ExpandCollapseCommand = new RelayCommand(ExpandCollapse); // todo: use proper type
        BrowseTwitchCommand = new RelayCommand(BrowseTwitch);
        PostCommand = new RelayCommand(Post, _ => Temp && !string.IsNullOrEmpty(Message));
        RemoveCommand = new RelayCommand(Remove);
        OpenPopupCommand = new RelayCommand(_ => IsPopupOpen = true);
        WhisperLeaderCommand = new RelayCommand(WhisperLeader);
        ToggleAutoPublicizeCommand = new RelayCommand(_ => WindowManager.ViewModels.LfgVM.ToggleAutoPublicizeCommand.Execute(null));

        _createdOn = DateTime.Now;
    }

    public Listing(ListingData l) : this()
    {
        LeaderName = l.LeaderName;
        LeaderId = l.LeaderId;
        ServerId = l.LeaderServerId;
        IsRaid = l.IsRaid;
        Message = l.Message;
        PlayerCount = l.PlayerCount;
        Temp = l.Temp;
    }

    public void UpdateIsMyLfg()
    {
        _dispatcher.InvokeAsync(() =>
        {
            IsMyLfg = Players.Any(x => x.PlayerId == Game.Me.PlayerId)
                    || LeaderId == Game.Me.PlayerId
                    || LeaderId == Game.Group.Leader.PlayerId;
        }, DispatcherPriority.DataBind);
    }

    void ExpandCollapse(object? force) // todo: use proper type
    {
        if (IsPopupOpen) return;

        if (force != null)
        {
            bool bForce = force is string s
                ? bool.TryParse(s, out var v) && v
                : (bool)force;

            if (bForce)
            {
                IsExpanded = !IsExpanded;
                if (!IsExpanded) return;

                WindowManager.ViewModels.LfgVM.LastClicked = this; // todo: maybe remove this reference (event?)
                StubInterface.Instance.StubClient.RequestPartyInfo(LeaderId, ServerId);
            }
            else
            {
                if (IsExpanded)
                {
                    IsExpanded = false;
                }
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
                WindowManager.ViewModels.LfgVM.LastClicked = this; // todo: maybe remove this reference (event?)
                StubInterface.Instance.StubClient.RequestPartyInfo(LeaderId, ServerId);
            }
        }
    }

    void BrowseTwitch()
    {
        if (!IsTwitch) return;

        Utils.Utilities.OpenUrl(TwitchLink);
    }

    void Post()
    {
        var msg = Message;
        var isRaid = IsRaid;

        if (Temp) WindowManager.ViewModels.LfgVM.Listings.Remove(this);
        StubInterface.Instance.StubClient.RegisterListing(msg, isRaid);
        Task.Delay(200).ContinueWith(_ => StubInterface.Instance.StubClient.RequestListings(App.Settings.LfgWindowSettings.MinLevel, App.Settings.LfgWindowSettings.MaxLevel));
    }

    void Remove()
    {
        if (Temp)
        {
            WindowManager.ViewModels.LfgVM.Listings.Remove(this);
        }
        else
        {
            WindowManager.ViewModels.LfgVM.RemoveMessageCommand.Execute(null);
        }
    }

    void WhisperLeader()
    {
        if (!Game.InGameChatOpen) FocusManager.SendNewLine();
        FocusManager.SendString($"/w {LeaderName} ");
    }
}