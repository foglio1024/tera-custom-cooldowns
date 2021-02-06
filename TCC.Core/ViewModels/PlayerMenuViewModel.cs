using Nostrum;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Analysis;
using TCC.Interop.Proxy;
using TCC.UI.Controls.Chat;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;
using FocusManager = TCC.UI.FocusManager;

namespace TCC.ViewModels
{
    public class PlayerMenuViewModel : TSPropertyChanged
    {
        private string _name = "";
        private string _info = "";
        private int _level;
        private Class _class = Class.Warrior;
        private uint _serverId;
        private ulong _gameId;
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
                if (_name == value) return;
                _name = value;
                N(nameof(Name));
                N(nameof(BlockLabelText));
                N(nameof(ShowAddFriend));
                N(nameof(ShowWhisper));
            }
        }

        public string Info
        {
            get => _info; set
            {
                if (_info == value) return; _info = value;
                N(nameof(Info));
            }
        }

        public int Level
        {
            get => _level; set
            {
                if (_level == value) return; _level = value; N(nameof(Level));
            }
        }

        public Class Class
        {
            get => _class;
            set
            {
                if (_class == value) return;
                _class = value;
                N(nameof(Class));
            }
        }

        public bool IsFromOtherServer
        {
            get => _isFromOtherServer;
            set
            {
                if (_isFromOtherServer == value) return;
                _isFromOtherServer = value;
                N();
                N(nameof(ShowAddFriend));
                N(nameof(ShowBlockUnblock));
                N(nameof(ShowWhisper));
            }
        }

        public bool ShowPartyInvite
        {
            get => _showPartyInvite;
            set
            {
                if (_showPartyInvite == value) return;
                _showPartyInvite = value;
                N();
            }
        }

        public bool ShowGuildInvite
        {
            get => _showGuildInvite;
            set
            {
                if (_showGuildInvite == value) return;
                _showGuildInvite = value;
                N();
            }
        }

        public bool ShowAddFriend => !IsBlocked && Name != Game.Me.Name && !IsFromOtherServer;
        public bool ShowBlockUnblock => !IsFromOtherServer;
        public bool ShowWhisper => !IsBlocked && !IsFromOtherServer;
        public string BlockLabelText => !IsBlocked ? Blocking ? "Are you sure?" : "Block" : "Unblock";
        public string FriendLabelText => IsFriend ? Unfriending ? "Are you sure?" : "Remove friend" : "Add friend";

        public string KickLabelText
        {
            get => _kickLabelText;
            set
            {
                if (_kickLabelText == value) return;
                _kickLabelText = value;
                N();
            }
        }

        public string GKickLabelText
        {
            get => _gkickLabelText;
            set
            {
                if (_gkickLabelText == value) return;
                _gkickLabelText = value;
                N();
            }
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

        public bool IsFriend => !Game.FriendList.FirstOrDefault(x => x.Name == _name).Equals(default(FriendData));

        public bool ShowFpsUtils => StubInterface.Instance.IsStubAvailable
                                 && StubInterface.Instance.IsFpsModAvailable;

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
            set
            {
                if (_unfriending == value) return;
                _unfriending = value;
                N();
            }
        }

        public bool Blocking
        {
            get => _blocking;
            set
            {
                if (_blocking == value) return;
                _blocking = value;
                N();
            }
        }

        public bool Kicking
        {
            get => _kicking;
            set
            {
                if (_kicking == value) return;
                _kicking = value;
                N();
            }
        }

        public bool Gkicking
        {
            get => _gkicking;
            set
            {
                if (_gkicking == value) return;
                _gkicking = value;
                N();
            }
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
            Dispatcher = Dispatcher.CurrentDispatcher;
            Name = "";
            Info = "";
            Level = 0;
            MoongourdPopupViewModel = new MoongourdPopupViewModel();

            InspectCommand = new RelayCommand(_ =>
            {
                if (IsFromOtherServer)
                {
                    if (_gameId == 0) return;
                    StubInterface.Instance.StubClient.InspectUser(_gameId);
                }
                else
                {
                    StubInterface.Instance.StubClient.InspectUser(Name);
                }
            });
            WhisperCommand = new RelayCommand(_ =>
            {
                if (!Game.InGameChatOpen) FocusManager.SendNewLine();
                FocusManager.SendString($"/w {Name} ");
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
                    N(nameof(FriendLabelText));
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
                        StubInterface.Instance.StubClient.BlockUser(Name);
                        Game.BlockList.Add(Name);
                        try
                        {
                            var i = Game.FriendList.IndexOf(Game.FriendList.FirstOrDefault(x => x.Name == Name));
                            if (i != -1)
                                Game.FriendList.RemoveAt(i);
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
                    N(nameof(BlockLabelText));
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
                StubInterface.Instance.StubClient.GroupInviteUser(Name);
            });
            GrantInviteCommand = new RelayCommand(_ =>
            {
                if (!Game.Group.TryGetMember(Name, out var u)) return;
                StubInterface.Instance.StubClient.SetInvitePower(u!.ServerId, u!.PlayerId, !u!.CanInvite);
                u.CanInvite = !u.CanInvite;
            });
            DelegateLeaderCommand = new RelayCommand(_ =>
            {
                if (!Game.Group.TryGetMember(Name, out var u)) return;
                StubInterface.Instance.StubClient.DelegateLeader(u!.ServerId, u!.PlayerId);
            });
            GroupKickCommand = new RelayCommand(_ =>
            {
                if (Kicking)
                {
                    if (Game.Group.TryGetMember(Name, out var u))
                    {
                        StubInterface.Instance.StubClient.KickUser(u!.ServerId, u!.PlayerId);
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
                MoongourdPopupViewModel.RequestInfo(Name, App.Settings.LastLanguage);
            });

            _win = new PlayerMenuWindow(this);

            PacketAnalyzer.ProcessorReady += () => PacketAnalyzer.Processor.Hook<S_ANSWER_INTERACTIVE>(OnAnswerInteractive);
        }

        public void Open(string name, uint serverId, int level = 0, Class cl = Class.None, ulong gameId = 0)
        {
            if (!App.Settings.EnablePlayerMenu) return;

            Dispatcher.InvokeAsync(() =>
            {
                Name = name;
                _serverId = serverId;
                _gameId = gameId;
                Class = cl;
                Info = Class.ToString();
                Level = level;
                _win.ShowAndPosition();
                IsFromOtherServer = serverId != Game.Server.ServerId;
                if (IsFromOtherServer)
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        ShowGuildInvite = false;
                        ShowPartyInvite = false;

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
            Dispatcher.InvokeAsync(() =>
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

            Dispatcher.InvokeAsync(() =>
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
            N(nameof(ShowPartyInvite));
            N(nameof(ShowGuildInvite));
            N(nameof(ShowAddFriend));
            N(nameof(ShowKick));
            N(nameof(ShowWhisper));
            N(nameof(BlockLabelText));
            N(nameof(FriendLabelText));
            N(nameof(IsBlocked));
            N(nameof(IsFriend));
            N(nameof(PowersLabelText));
            N(nameof(ShowDelegateLeader));
            N(nameof(ShowGrantPowers));
            N(nameof(ShowFpsUtils));
            N(nameof(ShowMakeGuildMaster));
            N(nameof(ShowGuildKick));
            N(nameof(ShowSeparator1));
            N(nameof(ShowSeparator2));
        }

        private void AskInteractive()
        {
            if (_serverId == 0 || string.IsNullOrEmpty(Name) || !StubInterface.Instance.IsStubAvailable) return;

            StubInterface.Instance.StubClient.AskInteractive(_serverId, Name);
        }
    }
}