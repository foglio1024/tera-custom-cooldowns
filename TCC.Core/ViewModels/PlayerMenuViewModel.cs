using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Windows.Input;
using TCC.Interop.Proxy;
using TCC.UI.Controls.Chat;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.ViewModels;

public class PlayerMenuViewModel : ThreadSafeObservableObject
{
    private string _name = "";
    private string _info = "";
    private int _level;
    private Class _class = Class.Warrior;
    private uint _serverId;
    private bool _showPartyInvite;
    private bool _showGuildInvite;
    private bool _isFromOtherServer;
    private bool _unfriending;
    private bool _blocking;
    private bool _kicking;
    private bool _gkicking;
    private uint _playerId;

    private string _kickLabelText = "Kick";
    private string _gkickLabelText = "Kick from guild";
    private readonly PlayerMenuWindow _win;
    public MoongourdPopupViewModel MoongourdPopupViewModel { get; }

    public event Action? UnfriendConfirmationRequested;
    public event Action? BlockConfirmationRequested;
    public event Action? KickConfirmationRequested;
    public event Action? GKickConfirmationRequested;

    public string Name
    {
        get => _name; set
        {
            if (!RaiseAndSetIfChanged(value, ref _name)) return;

            InvokePropertyChanged(nameof(BlockLabelText));
            InvokePropertyChanged(nameof(ShowAddFriend));
            InvokePropertyChanged(nameof(ShowWhisper));
            InvokePropertyChanged(nameof(ShowBlockUnblock));
        }
    }

    public string Info
    {
        get => _info;
        set => RaiseAndSetIfChanged(value, ref _info);

    }

    public int Level
    {
        get => _level; 
        set => RaiseAndSetIfChanged(value, ref _level);
    }

    public Class Class
    {
        get => _class;
        set => RaiseAndSetIfChanged(value, ref _class);
    }

    public bool IsFromOtherServer
    {
        get => _isFromOtherServer;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _isFromOtherServer)) return;
            InvokePropertyChanged(nameof(ShowAddFriend));
            InvokePropertyChanged(nameof(ShowBlockUnblock));
            InvokePropertyChanged(nameof(ShowWhisper));
        }
    }

    public bool ShowPartyInvite
    {
        get => _showPartyInvite;
        set => RaiseAndSetIfChanged(value, ref _showPartyInvite);
    }

    public bool ShowGuildInvite
    {
        get => _showGuildInvite;
        set => RaiseAndSetIfChanged(value, ref _showGuildInvite);
    }

    public bool ShowAddFriend => !IsBlocked && Name != Game.Me.Name && !IsFromOtherServer;
    public bool ShowBlockUnblock => Name != Game.Me.Name /*!IsFromOtherServer*/;
    public bool ShowWhisper => !IsBlocked && Name != Game.Me.Name/*&& !IsFromOtherServer*/;
    public string BlockLabelText => !IsBlocked ? Blocking ? "Are you sure?" : "Block" : "Unblock";
    public string FriendLabelText => IsFriend ? Unfriending ? "Are you sure?" : "Remove friend" : "Add friend";

    public string KickLabelText
    {
        get => _kickLabelText;
        set => RaiseAndSetIfChanged(value, ref _kickLabelText);
    }

    public string GKickLabelText
    {
        get => _gkickLabelText;
        set => RaiseAndSetIfChanged(value, ref _gkickLabelText);
    }

    public string PowersLabelText => !Game.Group.HasPowers(Name) ?
        "Grant invite power" : "Revoke invite power";

    public bool ShowSeparator1 => ShowKick || ShowDelegateLeader || ShowGrantPowers || ShowPartyInvite;
    public bool ShowSeparator2 => ShowGuildKick || ShowMakeGuildMaster || ShowGuildInvite;

    public bool ShowGrantPowers => Game.Group.IsRaid
                                   && Game.Group.AmILeader
                                   && Game.Group.Has(Name)
                                   && Game.Me.Name != Name;

    public bool ShowKick => Game.Group.Has(Name)
                            && Game.Me.Name != Name;

    public bool ShowDelegateLeader => Game.Group.AmILeader
                                      && Game.Group.Has(Name)
                                      && Game.Me.Name != Name;

    public bool IsBlocked => _name != "" && Game.BlockList.Contains(_name);

    public bool IsFriend => Game.Friends.Has(_name);

    public bool ShowFpsUtils => StubInterface.Instance.IsStubAvailable
                                && StubInterface.Instance.IsFpsModAvailable
                                && Game.Me.Name != Name;

    public bool ShowMakeGuildMaster => Game.Guild.AmIMaster &&
                                       Game.Guild.Has(Name);

    public bool ShowGuildKick => Game.Guild.AmIMaster &&
                                 Game.Guild.Has(Name);

    public bool ShowMgButtons => App.Settings.LastLanguage == "NA" ||
                                 App.Settings.LastLanguage == "RU" ||
                                 App.Settings.LastLanguage.StartsWith("EU");

    public bool Unfriending
    {
        get => _unfriending;
        set => RaiseAndSetIfChanged(value, ref _unfriending);
    }

    public bool Blocking
    {
        get => _blocking;
        set => RaiseAndSetIfChanged(value, ref _blocking);
    }

    public bool Kicking
    {
        get => _kicking;
        set => RaiseAndSetIfChanged(value, ref _kicking);
    }

    public bool Gkicking
    {
        get => _gkicking;
        set => RaiseAndSetIfChanged(value, ref _gkicking);
    }

    public ICommand InspectCommand { get; }
    public ICommand WhisperCommand { get; }
    public ICommand AddRemoveFriendCommand { get; }
    public ICommand BlockUnblockCommand { get; }
    public ICommand GroupInviteCommand { get; }
    public ICommand GrantInviteCommand { get; }
    public ICommand DelegateLeaderCommand { get; }
    public ICommand GroupKickCommand { get; }
    public ICommand GuildInviteCommand { get; }
    public ICommand MakeGuildMasterCommand { get; }
    public ICommand GuildKickCommand { get; }
    public ICommand HideShowPlayerCommand { get; }
    public ICommand OpenMoongourdPopupCommand { get; }

    public PlayerMenuViewModel()
    {
        Name = "";
        Info = "";
        Level = 0;
        MoongourdPopupViewModel = new MoongourdPopupViewModel();

        InspectCommand = new RelayCommand(_ =>
        {
            //if (IsFromOtherServer)
            //{
            //    if (_gameId == 0) return;
            //    StubInterface.Instance.StubClient.InspectUser(_gameId);
            //}
            StubInterface.Instance.StubClient.InspectUser(Name, _serverId);
        });
        WhisperCommand = new RelayCommand(_ =>
        {
            if (!Game.InGameChatOpen) FocusManager.SendNewLine();
            if (IsFromOtherServer)
            {
                var server = PacketAnalyzer.ServerDatabase.GetServer(_serverId);
                FocusManager.SendString($"/w {Name}@({server.Region})-{server.Name} ");
            }
            else
            {
                FocusManager.SendString($"/w {Name} ");
            }
        });
        AddRemoveFriendCommand = new RelayCommand(_ =>
        {
            if (IsFriend)
            {
                if (Unfriending)
                {
                    Close();
                    if (PacketAnalyzer.Factory!.ReleaseVersion / 100 >= 103)
                    {
                        StubInterface.Instance.StubClient.UnfriendUser(_playerId);
                    }
                    else
                    {
                        StubInterface.Instance.StubClient.UnfriendUser(Name);
                    }
                }
                else
                {
                    UnfriendConfirmationRequested?.Invoke();
                }
                Unfriending = !Unfriending;
                InvokePropertyChanged(nameof(FriendLabelText));
            }
            else
            {
                Close();
                new FriendMessageDialog().Show();
            }

            Refresh();
        });
        BlockUnblockCommand = new RelayCommand(_ =>
        {
            if (!IsBlocked)
            {
                if (Blocking)
                {
                    Close();
                    if (_serverId == 0)
                    {
                        Log.N("Error", "Failed to block user: server id cannot be 0.", NotificationType.Error);
                        Blocking = !Blocking;
                        InvokePropertyChanged(nameof(BlockLabelText));
                        Refresh();
                        return;
                    }
                    StubInterface.Instance.StubClient.BlockUser(Name, _serverId);
                    Game.BlockList.Add(Name);
                    try
                    {
                        Game.Friends.Remove(Name);
                    }
                    catch (Exception e)
                    {
                        Log.F($"Failed to remove friend while blocking: {e}");
                    }
                }
                else
                {
                    BlockConfirmationRequested?.Invoke();
                }
                Blocking = !Blocking;
                InvokePropertyChanged(nameof(BlockLabelText));
            }
            else
            {
                StubInterface.Instance.StubClient.UnblockUser(Name);
                Game.BlockList.Remove(Name);
                Close();
            }

            Refresh();
        });
        GroupInviteCommand = new RelayCommand(_ =>
        {
            StubInterface.Instance.StubClient.GroupInviteUser(Name, Game.Group.IsRaid);
        });
        GrantInviteCommand = new RelayCommand(_ =>
        {
            if (!Game.Group.TryGetMember(Name, out var u)) return;
            StubInterface.Instance.StubClient.SetInvitePower(u.ServerId, u.PlayerId, !u.CanInvite);
            u.CanInvite = !u.CanInvite;
        });
        DelegateLeaderCommand = new RelayCommand(_ =>
        {
            if (!Game.Group.TryGetMember(Name, out var u)) return;
            StubInterface.Instance.StubClient.DelegateLeader(u.ServerId, u.PlayerId);
        });
        GroupKickCommand = new RelayCommand(_ =>
        {
            if (Kicking)
            {
                if (Game.Group.TryGetMember(Name, out var u))
                {
                    StubInterface.Instance.StubClient.KickUser(u.ServerId, u.PlayerId);
                }
                Close();
            }
            else
            {
                KickLabelText = "Are you sure?";
                KickConfirmationRequested?.Invoke();
            }
            Kicking = !Kicking;
        });
        GuildInviteCommand = new RelayCommand(_ =>
        {
            StubInterface.Instance.StubClient.GuildInviteUser(Name);
        });
        MakeGuildMasterCommand = new RelayCommand(_ =>
        {
            if (!Game.InGameChatOpen) FocusManager.SendNewLine();
            FocusManager.SendString($"/gmaster {Name}");
        });
        GuildKickCommand = new RelayCommand(_ =>
        {
            if (Gkicking)
            {
                Close();
                if (!Game.InGameChatOpen) FocusManager.SendNewLine();
                FocusManager.SendString($"/gkick {Name}");
            }
            else
            {
                GKickLabelText = "Are you sure?";
                GKickConfirmationRequested?.Invoke();
            }
            Gkicking = !Gkicking;
        });
        HideShowPlayerCommand = new RelayCommand(cmd =>
        {
            StubInterface.Instance.StubClient.InvokeCommand($"fps {cmd} {Name}");
        }, _ => StubInterface.Instance.IsStubAvailable && StubInterface.Instance.IsFpsModAvailable);
        OpenMoongourdPopupCommand = new RelayCommand(_ =>
        {
            MoongourdPopupViewModel.RequestInfo(Name, Game.Server.Region);
        });

        _win = new PlayerMenuWindow(this);

        PacketAnalyzer.ProcessorReady += () => PacketAnalyzer.Processor.Hook<S_ANSWER_INTERACTIVE>(OnAnswerInteractive);
    }

    public void Open(string name, uint serverId, int level = 0, Class cl = Class.None)
    {
        if (!App.Settings.EnablePlayerMenu) return;

        _dispatcher.InvokeAsync(() =>
        {
            Name = name;
            _serverId = serverId;
            Class = cl;
            Info = Class.ToString();
            Level = level;
            _win.ShowAndPosition();
            IsFromOtherServer = serverId != Game.Server.ServerId;
            if (IsFromOtherServer)
            {
                _dispatcher.InvokeAsync(() =>
                {
                    ShowGuildInvite = false;
                    //ShowPartyInvite = false;

                    Refresh();
                    _win.ShowAndPosition();
                    _win.AnimateOpening();
                });
            }
            else
            {
                AskInteractive();
            }
        });
    }

    public void Close()
    {
        _dispatcher.InvokeAsync(() =>
        {
            _win.Close();
        });
    }

    public void Reset()
    {
        KickLabelText = "Kick";
        GKickLabelText = "Kick from guild";
        Gkicking = false;
        Kicking = false;
        Blocking = false;
        Unfriending = false;
    }

    private void OnAnswerInteractive(S_ANSWER_INTERACTIVE x)
    {
        if (!App.Settings.EnablePlayerMenu) return;

        _dispatcher.InvokeAsync(() =>
        {
            Game.DB!.MonsterDatabase.TryGetMonster((uint)x.TemplateId, 0, out var m);
            Name = x.Name;
            _playerId = x.PlayerId;
            Info = m.Name;
            Level = x.Level;
            Class = TccUtils.ClassFromModel((uint)x.TemplateId);
            ShowGuildInvite = x.Name != Game.Me.Name && !x.HasGuild;
            ShowPartyInvite = x.Name != Game.Me.Name && !x.HasParty;

            Refresh();
            _win.ShowAndPosition();
            _win.AnimateOpening();
        });
    }

    private void Refresh()
    {
        InvokePropertyChanged(nameof(ShowPartyInvite));
        InvokePropertyChanged(nameof(ShowGuildInvite));
        InvokePropertyChanged(nameof(ShowAddFriend));
        InvokePropertyChanged(nameof(ShowKick));
        InvokePropertyChanged(nameof(ShowWhisper));
        InvokePropertyChanged(nameof(BlockLabelText));
        InvokePropertyChanged(nameof(FriendLabelText));
        InvokePropertyChanged(nameof(IsBlocked));
        InvokePropertyChanged(nameof(IsFriend));
        InvokePropertyChanged(nameof(PowersLabelText));
        InvokePropertyChanged(nameof(ShowDelegateLeader));
        InvokePropertyChanged(nameof(ShowGrantPowers));
        InvokePropertyChanged(nameof(ShowFpsUtils));
        InvokePropertyChanged(nameof(ShowMakeGuildMaster));
        InvokePropertyChanged(nameof(ShowGuildKick));
        InvokePropertyChanged(nameof(ShowSeparator1));
        InvokePropertyChanged(nameof(ShowSeparator2));
    }

    private void AskInteractive()
    {
        if (_serverId == 0 || string.IsNullOrEmpty(Name) || !StubInterface.Instance.IsStubAvailable) return;

        StubInterface.Instance.StubClient.AskInteractive(_serverId, Name);
    }
}