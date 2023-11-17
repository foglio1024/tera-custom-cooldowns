using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets;

//TODO: remove all references from other vms to this, maybe move party members to Core
[TccModule]
[UsedImplicitly]
public class GroupWindowViewModel : TccWindowViewModel
{
    bool _raid;
    bool _firstCheck = true;
    readonly object _lock = new();
    bool _leaderOverride;
    ulong _aggroHolder;

    public event Action? SettingsUpdated;

    public ThreadSafeObservableCollection<User> Members { get; }
    public GroupWindowLayout GroupWindowLayout => ((GroupWindowSettings)Settings!).Layout;

    public ICollectionViewLiveShaping All { get; }
    public ICollectionViewLiveShaping Dps { get; }
    public ICollectionViewLiveShaping Tanks { get; }
    public ICollectionViewLiveShaping Healers { get; }
    public bool Raid
    {
        get => _raid;
        set
        {
            if (_raid == value) return;
            _raid = value;
            N();
        }
    }
    public int Size => Members.Count;
    public int ReadyCount => Members.Count(x => x.Ready == ReadyStatus.Ready);
    public int AliveCount => Members.Count(x => x.Alive);
    public bool Formed => Size > 0;
    public bool ShowDetails => Formed && ((GroupWindowSettings)Settings!).ShowDetails;
    public bool ShowLeaveButton => Formed && StubInterface.Instance.IsStubAvailable;
    public bool ShowLeaderButtons => Formed && StubInterface.Instance.IsStubAvailable && AmILeader;
    public bool Rolling { get; set; }

    public ICommand ShowLootWindowCommand { get; }

    public GroupWindowViewModel(GroupWindowSettings settings) : base(settings)
    {
        Members = new ThreadSafeObservableCollection<User>(_dispatcher);
        Members.CollectionChanged += Members_CollectionChanged;

        Dps = CollectionViewFactory.CreateLiveCollectionView(Members,
                  dps => dps is { Role: Role.Dps, Visible: true },
                  [nameof(User.Role), nameof(User.Visible)],
                  [new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)])
              ?? throw new Exception("Failed to create LiveCollectionView");
        Tanks = CollectionViewFactory.CreateLiveCollectionView(Members,
                    tank => tank is { Role: Role.Tank, Visible: true },
                    [nameof(User.Role), nameof(User.Visible)],
                    [new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)])
                ?? throw new Exception("Failed to create LiveCollectionView");
        Healers = CollectionViewFactory.CreateLiveCollectionView(Members,
                      healer => healer is { Role: Role.Healer, Visible: true },
                      [nameof(User.Role), nameof(User.Visible)],
                      [new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)])
                  ?? throw new Exception("Failed to create LiveCollectionView");
        All = CollectionViewFactory.CreateLiveCollectionView(Members,
                  user => user.Visible,
                  [nameof(User.Visible)],
                  [
                      new SortDescription(nameof(User.Role), ListSortDirection.Descending),
                      new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)
                  ])
              ?? throw new Exception("Failed to create LiveCollectionView");


        Game.Teleported += OnTeleported;
        Game.EncounterChanged += OnEncounterChanged;
        settings.SettingsUpdated += NotifySettingUpdated;
        settings.ThresholdChanged += NotifyThresholdChanged;
        settings.IgnoreMeChanged += ToggleMe;
        settings.LayoutChanged += OnLayoutChanged;

        ShowLootWindowCommand = new RelayCommand(Game.ShowLootDistributionWindow);
    }

    void OnEncounterChanged()
    {
        if (!Game.Encounter)
            SetAggro(0);
    }

    void OnLayoutChanged()
    {
        N(nameof(GroupWindowLayout));
        N(nameof(All));
        N(nameof(Dps));
        N(nameof(Healers));
        N(nameof(Tanks));
    }

    void OnTeleported()
    {
        if (!Game.CivilUnrestZone)
            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
        else
            PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
        Members.ToSyncList().ForEach(u => u.InRange = false);
    }


    void Members_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        //Task.Delay(0).ContinueWith(t =>
        //{
        //});
        N(nameof(Size));
        N(nameof(Formed));
        N(nameof(AmILeader));
        N(nameof(ShowDetails));
        N(nameof(ShowLeaveButton));
        N(nameof(AliveCount));
        N(nameof(ReadyCount));
        N(nameof(ShowLeaderButtons));
    }
    void NotifySettingUpdated()
    {
        SettingsUpdated?.Invoke();

        N(nameof(ShowDetails));
    }
    bool Exists(ulong id)
    {
        return Members.ToSyncList().Any(x => x.EntityId == id);
    }
    bool Exists(string name)
    {
        return Members.ToSyncList().Any(x => x.Name == name);
    }
    bool Exists(uint pId, uint sId)
    {
        return Members.ToSyncList().Any(x => x.PlayerId == pId && x.ServerId == sId);
    }

    public bool TryGetUser(string name, [MaybeNullWhen(false)] out User u)
    {
        var exists = Exists(name);
        u = exists ? Members.ToSyncList().FirstOrDefault(x => x.Name == name) : new User(_dispatcher);
        return exists;
    }
    public bool TryGetUser(ulong id, [MaybeNullWhen(false)] out User u)
    {
        var exists = Exists(id);

        u = exists ? Members.ToSyncList().FirstOrDefault(x => x.EntityId == id) : new User(_dispatcher);
        return u != null;
    }
    public bool TryGetUser(uint pId, uint sId, [MaybeNullWhen(false)] out User u)
    {
        var exists = Exists(pId, sId);
        u = exists ? Members.ToSyncList().FirstOrDefault(x => x.PlayerId == pId && x.ServerId == sId) : new User(_dispatcher);
        return exists;
    }

    public bool IsLeader(string name)
    {
        return Members.FirstOrDefault(x => x.Name == name)?.IsLeader ?? false;
    }
    public bool AmILeader => IsLeader(Game.Me.Name) || _leaderOverride;

    public void SetAggro(ulong target)
    {
        if (target == 0)
        {
            foreach (var item in Members.ToSyncList())
            {
                item.HasAggro = false;
            }
            return;
        }

        if (_aggroHolder == target) return;
        _aggroHolder = target;
        foreach (var item in Members.ToSyncList())
        {
            item.HasAggro = item.EntityId == target;
        }
    }
    void SetAggroCircle(AggroCircle circle, AggroAction action, ulong user)
    {
        if (WindowManager.ViewModels.NpcVM.CurrentHHphase != HarrowholdPhase.None) return;

        if (circle != AggroCircle.Main) return;
        if (action == AggroAction.Add)
        {
            SetAggro(user);
        }
    }
    internal void BeginOrRefreshAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
    {
        _dispatcher.InvokeAsync(() =>
        {
            if (ab.Infinity) duration = uint.MaxValue;
            var u = Members.ToSyncList().FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
            if (u == null) return;

            if (ab.Type is AbnormalityType.Buff or AbnormalityType.Special)
            {
                u.AddOrRefreshBuff(ab, duration, stacks);
                if (u.UserClass == Class.Warrior && ab.Id is >= 100200 and <= 100203)
                {
                    u.Role = Role.Tank; //def stance turned on: switch warrior to tank 
                }
            }
            else
            {
                // -- show only aggro stacks if we are in HH -- //
                if (WindowManager.ViewModels.NpcVM.CurrentHHphase >= HarrowholdPhase.Phase2)
                {
                    if (ab.Id != 950023 && ((GroupWindowSettings)Settings!).ShowOnlyAggroStacks) return;
                }
                // -------------------------------------------- //
                u.AddOrRefreshDebuff(ab, duration, stacks);
            }
        });
    }
    void EndAbnormality(Abnormality ab, uint playerId, uint serverId)
    {
        _dispatcher.InvokeAsync(() =>
        {

            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;

            if (ab.Type is AbnormalityType.Buff or AbnormalityType.Special)
            {
                u.RemoveBuff(ab);
                if (u.UserClass == Class.Warrior && ab.Id is >= 100200 and <= 100203)
                {
                    u.Role = Role.Dps; //def stance ended: make warrior dps again
                }
            }
            else
            {
                u.RemoveDebuff(ab);
            }
        });
    }

    void ClearAbnormality(uint playerId, uint serverId)
    {
        _dispatcher.Invoke(() =>
        {
            Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId)?.ClearAbnormalities();
        });
    }
    [Obsolete]
    public void AddOrUpdateMember(User p)
    {
        if (((GroupWindowSettings)Settings!).IgnoreMe && p.IsPlayer)
        {
            _leaderOverride = p.IsLeader;
        }
        lock (_lock) //TODO: really needed?
        {
            var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (user == null)
            {
                Members.Add(p);
                SendAddMessage(p.Name);
                p.Visible = !(((GroupWindowSettings)Settings).IgnoreMe && p.IsPlayer);

                return;
            }

            if (user.Online != p.Online) SendOnlineMessage(user.Name, p.Online);
            user.Online = p.Online;
            user.EntityId = p.EntityId;
            user.IsLeader = p.IsLeader;
            user.Order = p.Order;
            user.Awakened = p.Awakened;
            user.Visible = !(((GroupWindowSettings)Settings).IgnoreMe && p.IsPlayer);
        }
    }
    void AddOrUpdateMember(GroupMemberData p)
    {
        var visible = true;
        if (((GroupWindowSettings)Settings!).IgnoreMe && p.Name == Game.Me.Name)
        {
            _leaderOverride = p.IsLeader;
            visible = false;
        }
        var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
        if (user == null)
        {
            Members.Add(new User(p) { Visible = visible, InRange = Game.NearbyPlayers.ContainsKey(p.EntityId) });
            SendAddMessage(p.Name);
            return;
        }

        if (user.Online != p.Online) SendOnlineMessage(user.Name, p.Online);

        user.Online = p.Online;
        user.EntityId = p.EntityId;
        user.IsLeader = p.IsLeader;
        user.Order = p.Order;
        user.Awakened = p.Awakened;
        user.Visible = visible;
        user.MaxSt = p.MaxST;
        user.CurrentSt = p.CurrentST;
        //}
    }

    static void SendOnlineMessage(string name, bool newVal)
    {
        SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{name}", newVal ? "TCC_PARTY_MEMBER_LOGON" : "TCC_PARTY_MEMBER_LOGOUT");
    }

    void SendAddMessage(string name)
    {
        string msg;
        string opcode;
        if (Raid)
        {
            msg = "@0\vPartyPlayerName\v" + name;
            opcode = "SMT_JOIN_UNIONPARTY_PARTYPLAYER";
        }
        else
        {
            opcode = "SMT_JOIN_PARTY_PARTYPLAYER";
            msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
        }
        SystemMessagesProcessor.AnalyzeMessage(msg, opcode);
    }
    void SendDeathMessage(string name)
    {
        var msg = Raid ? $"@0\vPartyPlayerName\v{name}" : $"@0\vPartyPlayerName\v{name}\vparty\vparty";
        SystemMessagesProcessor.AnalyzeMessage(msg, "SMT_BATTLE_PARTY_DIE");
    }
    void SendLeaveMessage(string name)
    {
        string msg;
        string opcode;
        if (Raid)
        {
            opcode = "SMT_LEAVE_UNIONPARTY_PARTYPLAYER";
            msg = "@0\vPartyPlayerName\v" + name;
        }
        else
        {
            opcode = "SMT_LEAVE_PARTY_PARTYPLAYER";
            msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
        }
        SystemMessagesProcessor.AnalyzeMessage(msg, opcode);

    }
    void RemoveMember(uint playerId, uint serverId, bool kick = false)
    {
        var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
        if (u == null) return;
        u.ClearAbnormalities();
        Members.Remove(u);
        if (!kick) SendLeaveMessage(u.Name);
    }
    void ClearAll()
    {
        if (!((GroupWindowSettings)Settings!).Enabled || !_dispatcher.Thread.IsAlive) return;
        Members.ToSyncList().ForEach(x => x.ClearAbnormalities());
        Members.Clear();
        Raid = false;
        _leaderOverride = false;
    }
    void LogoutMember(uint playerId, uint serverId)
    {
        var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
        if (u == null) return;
        SendOnlineMessage(u.Name, false);
        u.Online = false;
    }
    void ToggleMe()
    {
        var me = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
        if (me == null) return;
        me.Visible = !((GroupWindowSettings)Settings!).IgnoreMe;
    }
    void ClearAllAbnormalities()
    {
        foreach (var x in Members.ToSyncList())
        {
            foreach (var b in x.Buffs.ToSyncList())
            {
                b.Dispose();
            }
            x.Buffs.Clear();
            foreach (var b in x.Debuffs.ToSyncList())
            {
                b.Dispose();
            }
            x.Debuffs.Clear();
        }
    }
    void SetNewLeader(string name)
    {
        foreach (var m in Members.ToSyncList())
        {
            m.IsLeader = m.Name == name;
        }
        _leaderOverride = name == Game.Me.Name;
        N(nameof(AmILeader));
        N(nameof(ShowLeaderButtons));
    }

    public void StartRoll()
    {
        Rolling = true;
        //Members.ToList().ForEach(u => u.IsRolling = true);
        foreach (var m in Members.ToSyncList())
        {
            m.IsRolling = true;
        }
    }
    void SetRoll(ulong entityId, int rollResult)
    {
        if (rollResult == int.MaxValue) rollResult = -1;
        Members.ToSyncList().ForEach(member =>
        {
            if (member.EntityId == entityId)
            {
                member.RollResult = rollResult;
            }
            member.IsWinning = member.EntityId == GetWinningUser() && member.RollResult != -1;
        });
    }
    void SetRoll(uint serverId, uint playerId, int rollResult)
    {
        if (rollResult == int.MaxValue) rollResult = -1;
        Members.ToSyncList().ForEach(member =>
        {
            if (member.PlayerId == playerId && member.ServerId == serverId)
            {
                member.RollResult = rollResult;
            }
            member.IsWinning = member.EntityId == GetWinningUser() && member.RollResult != -1;
        });
    }
    void EndRoll()
    {
        Rolling = false;


        foreach (var m in Members.ToSyncList())
        {
            m.IsRolling = false;
            m.IsWinning = false;
            m.RollResult = 0;
        }
        //Members.ToList().ForEach(u =>
        //{
        //u.IsRolling = false;
        //u.IsWinning = false;
        //u.RollResult = 0;
        //});
    }
    ulong GetWinningUser()
    {
        return Members.ToSyncList().OrderByDescending(u => u.RollResult).First().EntityId;
        //Members.ToList().ForEach(user => user.IsWinning = user.EntityId == Members.OrderByDescending(u => u.RollResult).First().EntityId);
    }
    public void SetReadyStatus(ReadyPartyMember p)
    {

        if (_firstCheck)
        {
            foreach (var u in Members.ToSyncList())
            {
                u.Ready = ReadyStatus.Undefined;
            }
        }
        var user = Members.ToSyncList().FirstOrDefault(u => u.PlayerId == p.PlayerId && u.ServerId == p.ServerId);
        if (user != null) user.Ready = p.Status;
        _firstCheck = false;
        N(nameof(ReadyCount));
    }
    void EndReadyCheck()
    {
        Task.Delay(4000).ContinueWith(_ =>
        {
            //Members.ToList().ForEach(x => x.Ready = ReadyStatus.None);
            foreach (var u in Members.ToSyncList())
            {
                u.Ready = ReadyStatus.None;
            }
        });
        _firstCheck = true;
    }
    void UpdateMemberHp(uint playerId, uint serverId, int curHp, int maxHp)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.CurrentHp = curHp;
            u.MaxHp = maxHp;
        });
    }
    void UpdateMemberMp(uint playerId, uint serverId, int curMp, int maxMp)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.CurrentMp = curMp;
            u.MaxMp = maxMp;
        });
    }
    void UpdateMemberStamina(uint playerId, uint serverId, int curSt, int maxSt)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.CurrentSt = curSt;
            u.MaxSt = maxSt;
        });
    }
    void SetRaid(bool raid)
    {
        _dispatcher.InvokeAsync(new Action(() => Raid = raid));
    }
    void UpdateMember(GroupMemberData update)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var current = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == update.PlayerId && x.ServerId == update.ServerId);
            if (current == null) return;
            current.CurrentHp = update.CurrentHP;
            current.CurrentMp = update.CurrentMP;
            current.MaxHp = update.MaxHP;
            current.MaxMp = update.MaxMP;
            current.Level = update.Level;
            if (current.Alive && !update.Alive)
            {
                SendDeathMessage(current.Name);
                current.CurrentHp = 0; // force hp to 0 when ded
            }
            current.Alive = update.Alive;
            N(nameof(AliveCount));
            if (!update.Alive) current.HasAggro = false;
        });
    }
    void NotifyThresholdChanged()
    {
        N(nameof(Size));
    }

    void UpdateMemberLocation(uint playerId, uint serverId, int channel, uint continentId)
    {
        var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
        if (u == null) return;
        var ch = channel > 1000 ? "" : " ch." + channel;
        u.Location = Game.DB!.TryGetGuardOrDungeonNameFromContinentId(continentId, out var l) ? l + ch : "Unknown";
    }
    void UpdatePartyMemberAbnormality(uint playerId, uint serverId, uint id, uint duration, int stacks)
    {
        _dispatcher.InvokeAsync(() =>
        {
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(id, out var ab) || !ab.CanShow) return;
            BeginOrRefreshAbnormality(ab, stacks, duration, playerId, serverId);
        });
    }
    void EndPartyMemberAbnormality(uint playerId, uint serverId, uint id)
    {
        _dispatcher.InvokeAsync(() =>
        {
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(id, out var ab) || !ab.CanShow) return;
            EndAbnormality(ab, playerId, serverId);
        });
    }

    protected override void InstallHooks()
    {
        PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

        PacketAnalyzer.Processor.Hook<S_USER_EFFECT>(OnUserEffect);
        PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
        PacketAnalyzer.Processor.Hook<S_LEAVE_PARTY>(OnLeaveParty);
        PacketAnalyzer.Processor.Hook<S_BAN_PARTY>(OnBanParty);
        PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
        PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
        PacketAnalyzer.Processor.Hook<S_LOAD_TOPO>(OnLoadTopo);

        PacketAnalyzer.Processor.Hook<S_SPAWN_USER>(OnSpawnUser);
        PacketAnalyzer.Processor.Hook<S_DESPAWN_USER>(OnDespawnUser);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_BUFF_UPDATE>(OnPartyMemberBuffUpdate);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_ABNORMAL_ADD>(OnPartyMemberAbnormalAdd);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_ABNORMAL_DEL>(OnPartyMemberAbnormalDel);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_ABNORMAL_CLEAR>(OnPartyMemberAbnormalClear);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_ABNORMAL_REFRESH>(OnPartyMemberAbnormalRefresh);
        PacketAnalyzer.Processor.Hook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
        PacketAnalyzer.Processor.Hook<S_LEAVE_PARTY_MEMBER>(OnLeavePartyMember);
        PacketAnalyzer.Processor.Hook<S_BAN_PARTY_MEMBER>(OnBanPartyMember);
        PacketAnalyzer.Processor.Hook<S_LOGOUT_PARTY_MEMBER>(OnLogoutPartyMember);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_CHANGE_HP>(OnPartyMemberChangeHp);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_CHANGE_MP>(OnPartyMemberChangeMp);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_CHANGE_STAMINA>(OnPartyMemberChangeStamina);
        PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
        PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_STAT_UPDATE>(OnPartyMemberStatUpdate);
        PacketAnalyzer.Processor.Hook<S_CHECK_TO_READY_PARTY>(OnCheckToReadyParty);
        PacketAnalyzer.Processor.Hook<S_CHECK_TO_READY_PARTY_FIN>(OnCheckToReadyPartyFin);
        PacketAnalyzer.Processor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
        PacketAnalyzer.Processor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
        PacketAnalyzer.Processor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);

        PacketAnalyzer.Processor.Hook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
        PacketAnalyzer.Processor.Hook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
        PacketAnalyzer.Processor.Hook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
    }

    protected override void RemoveHooks()
    {
        PacketAnalyzer.Processor.Unhook<S_USER_EFFECT>(OnUserEffect);
        PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
        PacketAnalyzer.Processor.Unhook<S_LEAVE_PARTY>(OnLeaveParty);
        PacketAnalyzer.Processor.Unhook<S_BAN_PARTY>(OnBanParty);
        PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
        PacketAnalyzer.Processor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
        PacketAnalyzer.Processor.Unhook<S_LOAD_TOPO>(OnLoadTopo);
        PacketAnalyzer.Processor.Unhook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
        PacketAnalyzer.Processor.Unhook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
        PacketAnalyzer.Processor.Unhook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
        PacketAnalyzer.Processor.Unhook<S_SPAWN_USER>(OnSpawnUser);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_BUFF_UPDATE>(OnPartyMemberBuffUpdate);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_ABNORMAL_ADD>(OnPartyMemberAbnormalAdd);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_ABNORMAL_DEL>(OnPartyMemberAbnormalDel);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_ABNORMAL_CLEAR>(OnPartyMemberAbnormalClear);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_ABNORMAL_REFRESH>(OnPartyMemberAbnormalRefresh);
        PacketAnalyzer.Processor.Unhook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
        PacketAnalyzer.Processor.Unhook<S_LEAVE_PARTY_MEMBER>(OnLeavePartyMember);
        PacketAnalyzer.Processor.Unhook<S_BAN_PARTY_MEMBER>(OnBanPartyMember);
        PacketAnalyzer.Processor.Unhook<S_LOGOUT_PARTY_MEMBER>(OnLogoutPartyMember);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_CHANGE_HP>(OnPartyMemberChangeHp);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_CHANGE_MP>(OnPartyMemberChangeMp);
        PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_STAT_UPDATE>(OnPartyMemberStatUpdate);
        PacketAnalyzer.Processor.Unhook<S_CHECK_TO_READY_PARTY>(OnCheckToReadyParty);
        PacketAnalyzer.Processor.Unhook<S_CHECK_TO_READY_PARTY_FIN>(OnCheckToReadyPartyFin);
        PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
        PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
        PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_END>(OnAbnormalityEnd);

    }

    void OnDisconnected()
    {
        ClearAllAbnormalities();
    }

    void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (Game.IsMe(p.TargetId))
        {
            if (Size > ((GroupWindowSettings)Settings!).DisableAbnormalitiesThreshold) return;
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            if (p.Duration == int.MaxValue) ab.Infinity = true;

            BeginOrRefreshAbnormality(ab, p.Stacks, p.Duration, Game.Me.PlayerId, Game.Me.ServerId);
        }
        else
        {
            // fix for missing warrior stances
            //if (p.TargetId != p.CasterId) return;
            if (p.AbnormalityId is < 100101 or > 100203) return;
            //Game.NearbyPlayers.TryGetValue(p.TargetId, out var name);
            //Log.CW($"Starting stance on {p.TargetId} ({name}), members: {(Members.ToSyncList().Select(i => $"{i.EntityId} ({i.Name})").ToList().ToCSV())}");
            if (!TryGetUser(p.TargetId, out var u)) return;
            UpdatePartyMemberAbnormality(u.PlayerId, u.ServerId, p.AbnormalityId, p.Duration, p.Stacks);
        }
    }
    void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        if (Size > ((GroupWindowSettings)Settings!).DisableAbnormalitiesThreshold) return;
        if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
        if (p.Duration == int.MaxValue) ab.Infinity = true;

        BeginOrRefreshAbnormality(ab, p.Stacks, p.Duration, Game.Me.PlayerId, Game.Me.ServerId);
    }
    void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;

        if (Game.IsMe(p.TargetId))
        {
            EndAbnormality(ab, Game.Me.PlayerId, Game.Me.ServerId);
        }
        else
        {
            if (p.AbnormalityId is < 100101 or > 100203) return;
            //Game.NearbyPlayers.TryGetValue(p.TargetId, out var name);
            //Log.CW($"Ending stance on {p.TargetId} ({name}), members: {(Members.ToSyncList().Select(i => $"{i.EntityId} ({i.Name})").ToList().ToCSV())}");
            if (!TryGetUser(p.TargetId, out var u)) return;
            EndAbnormality(ab, u.PlayerId, u.ServerId);
        }
    }
    void OnCheckToReadyPartyFin(S_CHECK_TO_READY_PARTY_FIN p)
    {
        EndReadyCheck();
    }
    void OnCheckToReadyParty(S_CHECK_TO_READY_PARTY p)
    {
        _dispatcher.InvokeAsync(() => p.Party.ForEach(SetReadyStatus));
    }
    void OnPartyMemberStatUpdate(S_PARTY_MEMBER_STAT_UPDATE p)
    {
        UpdateMember(p.GroupMemberData);
    }
    void OnPartyMemberChangeMp(S_PARTY_MEMBER_CHANGE_MP p)
    {
        UpdateMemberMp(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
    }
    void OnPartyMemberChangeStamina(S_PARTY_MEMBER_CHANGE_STAMINA p)
    {
        UpdateMemberStamina(p.PlayerId, p.ServerId, p.CurrentST, p.MaxST);
    }
    void OnPartyMemberChangeHp(S_PARTY_MEMBER_CHANGE_HP p)
    {
        UpdateMemberHp(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
    }
    void OnLogoutPartyMember(S_LOGOUT_PARTY_MEMBER p)
    {
        LogoutMember(p.PlayerId, p.ServerId);
        ClearAbnormality(p.PlayerId, p.ServerId);
    }
    void OnBanPartyMember(S_BAN_PARTY_MEMBER p)
    {
        RemoveMember(p.PlayerId, p.ServerId, true);
    }
    void OnLeavePartyMember(S_LEAVE_PARTY_MEMBER p)
    {
        RemoveMember(p.PlayerId, p.ServerId);
    }
    void OnPartyMemberList(S_PARTY_MEMBER_LIST p)
    {
        SetRaid(p.Raid);
        _dispatcher.InvokeAsync(() =>
        {
            foreach (var member in p.Members) AddOrUpdateMember(member);
        });
    }
    void OnChangePartyManager(S_CHANGE_PARTY_MANAGER m)
    {
        SetNewLeader(m.Name);
    }
    void OnPartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH m)
    {
        UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
    }
    void OnPartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR m)
    {
        ClearAbnormality(m.PlayerId, m.ServerId);
    }
    void OnPartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL m)
    {
        EndPartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id);
    }
    void OnPartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD m)
    {
        UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
    }
    void OnPartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE m)
    {
        foreach (var buff in m.Abnormals) UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, buff.Id, buff.Duration, buff.Stacks);
    }
    void OnSpawnUser(S_SPAWN_USER m)
    {
        if (!TryGetUser(m.PlayerId, m.ServerId, out var u)) return;
        u.InRange = true;
        u.EntityId = m.EntityId;
        //UpdateMemberGear(m.PlayerId, m.ServerId, m.Weapon, m.Armor, m.Gloves, m.Boots);
    }
    void OnDespawnUser(S_DESPAWN_USER m)
    {
        if (!TryGetUser(m.EntityId, out var u)) return;
        u.InRange = false;
    }
    void OnResultItemBidding(S_RESULT_ITEM_BIDDING m)
    {
        EndRoll();
    }
    void OnResultBiddingDiceThrow(S_RESULT_BIDDING_DICE_THROW m)
    {
        if (!Rolling) StartRoll();
        if (m.EntityId != 0)
            SetRoll(m.EntityId, m.RollResult);
        else
            SetRoll(m.ServerId, m.PlayerId, m.RollResult);
    }
    void OnAskBiddingRareItem(S_ASK_BIDDING_RARE_ITEM m)
    {
        StartRoll();
    }

    void OnLoadTopo(S_LOAD_TOPO m)
    {
        ClearAllAbnormalities();
        SetAggro(0);
    }
    void OnReturnToLobby(S_RETURN_TO_LOBBY m)
    {
        ClearAll();
    }
    void OnLogin(S_LOGIN m)
    {
        ClearAll();
    }
    void OnBanParty(S_BAN_PARTY m)
    {
        ClearAll();
    }
    void OnLeaveParty(S_LEAVE_PARTY m)
    {
        ClearAll();
    }
    void OnGetUserList(S_GET_USER_LIST m)
    {
        ClearAll();
    }
    void OnUserEffect(S_USER_EFFECT m)
    {
        SetAggroCircle(m.Circle, m.Action, m.User);
    }
    void OnPartyMemberIntervalPosUpdate(S_PARTY_MEMBER_INTERVAL_POS_UPDATE p)
    {
        UpdateMemberLocation(p.PlayerId, p.ServerId, p.Channel, p.ContinentId);
    }
    void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
    {
        UpdateMemberStamina(Game.Me.PlayerId, Game.Me.ServerId, p.CurrentST, Game.Me.MaxST);
    }
}

public record struct DropItem(GameId GameId, uint ItemId, uint Amount);