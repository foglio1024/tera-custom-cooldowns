using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Nostrum;
using Nostrum.Factories;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Settings.WindowSettings;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    //TODO: remove all references from other vms to this, maybe move party members to Core
    [TccModule]
    public class GroupWindowViewModel : TccWindowViewModel
    {
        private bool _raid;
        private bool _firstCheck = true;
        private readonly object _lock = new object();
        private bool _leaderOverride;
        private ulong _aggroHolder;

        public event Action SettingsUpdated;

        public TSObservableCollection<User> Members { get; }
        public GroupWindowLayout GroupWindowLayout => ((GroupWindowSettings)Settings).Layout;

        public ICollectionViewLiveShaping All { get; }
        public ICollectionViewLiveShaping Dps { [UsedImplicitly] get; }
        public ICollectionViewLiveShaping Tanks { [UsedImplicitly] get; }
        public ICollectionViewLiveShaping Healers { [UsedImplicitly] get; }
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
        public bool ShowDetails => Formed && ((GroupWindowSettings)Settings).ShowDetails;
        public bool ShowLeaveButton => Formed && ProxyInterface.Instance.IsStubAvailable;
        public bool ShowLeaderButtons => Formed && ProxyInterface.Instance.IsStubAvailable && AmILeader;
        public bool Rolling { get; set; }

        public GroupWindowViewModel(GroupWindowSettings settings) : base(settings)
        {
            Members = new TSObservableCollection<User>(Dispatcher);
            Members.CollectionChanged += Members_CollectionChanged;

            Dps = CollectionViewFactory.CreateLiveCollectionView(Members,
                dps => dps.Role == Role.Dps && dps.Visible,
                new[] { nameof(User.Role), nameof(User.Visible) },
                new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Tanks = CollectionViewFactory.CreateLiveCollectionView(Members,
                tank => tank.Role == Role.Tank && tank.Visible,
                new[] { nameof(User.Role), nameof(User.Visible) },
                new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Healers = CollectionViewFactory.CreateLiveCollectionView(Members,
                healer => healer.Role == Role.Healer && healer.Visible,
                new[] { nameof(User.Role), nameof(User.Visible) },
                new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            All = CollectionViewFactory.CreateLiveCollectionView(Members,
                user => user.Visible,
                new[] { nameof(User.Visible) },
                new[]
                {
                    new SortDescription(nameof(User.Role), ListSortDirection.Descending),
                    new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)
                });

            Game.Teleported += OnTeleported;

            settings.SettingsUpdated += NotifySettingUpdated;
            settings.ThresholdChanged += NotifyThresholdChanged;
            settings.IgnoreMeChanged += ToggleMe;
            settings.LayoutChanged += OnLayoutChanged;
        }

        private void OnLayoutChanged()
        {
            N(nameof(GroupWindowLayout));
            N(nameof(All));
            N(nameof(Dps));
            N(nameof(Healers));
            N(nameof(Tanks));
        }

        private void OnTeleported()
        {
            if (!Game.CivilUnrestZone)
                PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
            else
                PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
        }


        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
        private void NotifySettingUpdated()
        {
            SettingsUpdated?.Invoke();

            N(nameof(ShowDetails));
        }
        private bool Exists(ulong id)
        {
            return Members.ToSyncList().Any(x => x.EntityId == id);
        }
        private bool Exists(string name)
        {
            return Members.ToSyncList().Any(x => x.Name == name);
        }
        private bool Exists(uint pId, uint sId)
        {
            return Members.ToSyncList().Any(x => x.PlayerId == pId && x.ServerId == sId);
        }

        public bool TryGetUser(string name, out User u)
        {
            u = Exists(name) ? Members.ToSyncList().FirstOrDefault(x => x.Name == name) : null;
            return Exists(name);
        }
        public bool TryGetUser(ulong id, out User u)
        {
            u = Exists(id) ? Members.ToSyncList().FirstOrDefault(x => x.EntityId == id) : null;
            return Exists(id);
        }
        public bool TryGetUser(uint pId, uint sId, out User u)
        {
            u = Exists(pId, sId) ? Members.ToSyncList().FirstOrDefault(x => x.PlayerId == pId && x.ServerId == sId) : null;
            return Exists(pId, sId);
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
        private void SetAggroCircle(AggroCircle circle, AggroAction action, ulong user)
        {
            if (WindowManager.ViewModels.NpcVM.CurrentHHphase != HarrowholdPhase.None) return;

            if (circle != AggroCircle.Main) return;
            if (action == AggroAction.Add)
            {
                SetAggro(user);
            }
        }
        private void BeginOrRefreshAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (ab.Infinity) duration = uint.MaxValue;
                var u = Members.ToSyncList().FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
                if (u == null) return;

                if (ab.Type == AbnormalityType.Buff || ab.Type == AbnormalityType.Special)
                {
                    u.AddOrRefreshBuff(ab, duration, stacks);
                    if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203)
                    {
                        u.Role = Role.Tank; //def stance turned on: switch warrior to tank 
                    }
                }
                else
                {
                    // -- show only aggro stacks if we are in HH -- //
                    if (WindowManager.ViewModels.NpcVM.CurrentHHphase >= HarrowholdPhase.Phase2)
                    {
                        if (ab.Id != 950023 && ((GroupWindowSettings)Settings).ShowOnlyAggroStacks) return;
                    }
                    // -------------------------------------------- //
                    u.AddOrRefreshDebuff(ab, duration, stacks);
                }
            });
        }
        private void EndAbnormality(Abnormality ab, uint playerId, uint serverId)
        {
            Dispatcher.InvokeAsync(() =>
            {

                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;

                if (ab.Type == AbnormalityType.Buff || ab.Type == AbnormalityType.Special)
                {
                    u.RemoveBuff(ab);
                    if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203)
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

        private void ClearAbnormality(uint playerId, uint serverId)
        {
            Dispatcher.Invoke(() =>
            {
                Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId)?.ClearAbnormalities();
            });
        }
        [Obsolete]
        public void AddOrUpdateMember(User p)
        {
            if (((GroupWindowSettings)Settings).IgnoreMe && p.IsPlayer)
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
        private void AddOrUpdateMember(GroupMemberData p)
        {
            var visible = true;
            if (((GroupWindowSettings)Settings).IgnoreMe && p.Name == Game.Me.Name)
            {
                _leaderOverride = p.IsLeader;
                visible = false;
            }
            var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (user == null)
            {
                Members.Add(new User(p) {Visible =  visible, InRange = Game.NearbyPlayers.ContainsKey(p.EntityId)});
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
            //}
        }

        private static void SendOnlineMessage(string name, bool newVal)
        {
            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{name}", newVal ? "TCC_PARTY_MEMBER_LOGON" : "TCC_PARTY_MEMBER_LOGOUT");
        }

        private void SendAddMessage(string name)
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
        private void SendDeathMessage(string name)
        {
            var msg = Raid ? $"@0\vPartyPlayerName\v{name}" : $"@0\vPartyPlayerName\v{name}\vparty\vparty";
            SystemMessagesProcessor.AnalyzeMessage(msg, "SMT_BATTLE_PARTY_DIE");
            //if (ProxyInterface.Instance.IsStubAvailable) ProxyInterface.Instance.Stub.ForceSystemMessage(msg, opcode);
        }
        private void SendLeaveMessage(string name)
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
        private void RemoveMember(uint playerId, uint serverId, bool kick = false)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.ClearAbnormalities();
            Members.Remove(u);
            if (!kick) SendLeaveMessage(u.Name);
        }
        private void ClearAll()
        {
            if (!((GroupWindowSettings)Settings).Enabled || !Dispatcher.Thread.IsAlive) return;
            Members.ToSyncList().ForEach(x => x.ClearAbnormalities());
            Members.Clear();
            Raid = false;
            _leaderOverride = false;
        }
        private void LogoutMember(uint playerId, uint serverId)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            SendOnlineMessage(u.Name, false);
            u.Online = false;
        }
        private void ToggleMe()
        {
            var me = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
            if (me == null) return;
            me.Visible = !((GroupWindowSettings)Settings).IgnoreMe;
        }
        private void ClearAllAbnormalities()
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
        private void SetNewLeader(string name)
        {
            foreach (var m in Members.ToSyncList())
            {
                m.IsLeader = m.Name == name;
            }
            _leaderOverride = name == Game.Me.Name;
            N(nameof(AmILeader));
            N(nameof(ShowLeaderButtons));
        }
        private void StartRoll()
        {
            Rolling = true;
            //Members.ToList().ForEach(u => u.IsRolling = true);
            foreach (var m in Members.ToSyncList())
            {
                m.IsRolling = true;
            }
        }
        private void SetRoll(ulong entityId, int rollResult)
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
            //var u = Members.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            //if (u == null) return;
            //u.RollResult = rollResult;
            //u.IsWinning = u.EntityId == GetWinningUser();
        }
        private void EndRoll()
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
        private ulong GetWinningUser()
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
        private void EndReadyCheck()
        {
            Task.Delay(4000).ContinueWith(t =>
            {
                //Members.ToList().ForEach(x => x.Ready = ReadyStatus.None);
                foreach (var u in Members.ToSyncList())
                {
                    u.Ready = ReadyStatus.None;
                }
            });
            _firstCheck = true;
        }
        private void UpdateMemberHp(uint playerId, uint serverId, int curHp, int maxHp)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentHp = curHp;
                u.MaxHp = maxHp;
            });
        }
        private void UpdateMemberMp(uint playerId, uint serverId, int curMp, int maxMp)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentMp = curMp;
                u.MaxMp = maxMp;
            });
        }
        private void SetRaid(bool raid)
        {
            Dispatcher.InvokeAsync(new Action(() => Raid = raid));
        }
        private void UpdateMember(GroupMemberData update)
        {
            Dispatcher.InvokeAsync(() =>
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
        private void NotifyThresholdChanged()
        {
            N(nameof(Size));
        }

        private void UpdateMemberLocation(uint playerId, uint serverId, int channel, uint continentId)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            var ch = channel > 1000 ? "" : " ch." + channel;
            u.Location = Game.DB.TryGetGuardOrDungeonNameFromContinentId(continentId, out var l) ? l + ch : "Unknown";
        }
        private void UpdatePartyMemberAbnormality(uint playerId, uint serverId, uint id, uint duration, int stacks)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (!AbnormalityUtils.Exists(id, out var ab) || !AbnormalityUtils.Pass(ab)) return;
                BeginOrRefreshAbnormality(ab, stacks, duration, playerId, serverId);
            });
        }
        private void EndPartyMemberAbnormality(uint playerId, uint serverId, uint id)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (!AbnormalityUtils.Exists(id, out var ab) || !AbnormalityUtils.Pass(ab)) return;
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
            PacketAnalyzer.Processor.Hook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
            PacketAnalyzer.Processor.Hook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
            PacketAnalyzer.Processor.Hook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
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
            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_STAT_UPDATE>(OnPartyMemberStatUpdate);
            PacketAnalyzer.Processor.Hook<S_CHECK_TO_READY_PARTY>(OnCheckToReadyParty);
            PacketAnalyzer.Processor.Hook<S_CHECK_TO_READY_PARTY_FIN>(OnCheckToReadyPartyFin);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);
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
        private void OnDisconnected()
        {
            ClearAllAbnormalities();
        }

        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            if (Size > ((GroupWindowSettings)Settings).DisableAbnormalitiesThreshold) return;
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            if (p.Duration == int.MaxValue) ab.Infinity = true;

            BeginOrRefreshAbnormality(ab, p.Stacks, p.Duration, Game.Me.PlayerId, Game.Me.ServerId);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            if (Size > ((GroupWindowSettings)Settings).DisableAbnormalitiesThreshold) return;
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            if (p.Duration == int.MaxValue) ab.Infinity = true;

            BeginOrRefreshAbnormality(ab, p.Stacks, p.Duration, Game.Me.PlayerId, Game.Me.ServerId);
        }
        private void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;

            EndAbnormality(ab, Game.Me.PlayerId, Game.Me.ServerId);
        }
        private void OnCheckToReadyPartyFin(S_CHECK_TO_READY_PARTY_FIN p)
        {
            EndReadyCheck();
        }
        private void OnCheckToReadyParty(S_CHECK_TO_READY_PARTY p)
        {
            Dispatcher.InvokeAsync(() => p.Party.ForEach(SetReadyStatus));
        }
        private void OnPartyMemberStatUpdate(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            UpdateMember(p.GroupMemberData);
        }
        private void OnPartyMemberChangeMp(S_PARTY_MEMBER_CHANGE_MP p)
        {
            UpdateMemberMp(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
        }
        private void OnPartyMemberChangeHp(S_PARTY_MEMBER_CHANGE_HP p)
        {
            UpdateMemberHp(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
        }
        private void OnLogoutPartyMember(S_LOGOUT_PARTY_MEMBER p)
        {
            LogoutMember(p.PlayerId, p.ServerId);
            ClearAbnormality(p.PlayerId, p.ServerId);
        }
        private void OnBanPartyMember(S_BAN_PARTY_MEMBER p)
        {
            RemoveMember(p.PlayerId, p.ServerId, true);
        }
        private void OnLeavePartyMember(S_LEAVE_PARTY_MEMBER p)
        {
            RemoveMember(p.PlayerId, p.ServerId);
        }
        private void OnPartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            SetRaid(p.Raid);
            Dispatcher.InvokeAsync(() => p.Members.ForEach(AddOrUpdateMember));
        }
        private void OnChangePartyManager(S_CHANGE_PARTY_MANAGER m)
        {
            SetNewLeader(m.Name);
        }
        private void OnPartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH m)
        {
            UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
        }
        private void OnPartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR m)
        {
            ClearAbnormality(m.PlayerId, m.ServerId);
        }
        private void OnPartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL m)
        {
            EndPartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id);
        }
        private void OnPartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD m)
        {
            UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
        }
        private void OnPartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE m)
        {
            foreach (var buff in m.Abnormals) UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, buff.Id, buff.Duration, buff.Stacks);
        }
        private void OnSpawnUser(S_SPAWN_USER m)
        {
            if (!TryGetUser(m.EntityId, out var u)) return;
            u.InRange = true;
            //UpdateMemberGear(m.PlayerId, m.ServerId, m.Weapon, m.Armor, m.Gloves, m.Boots);
        }
        private void OnDespawnUser(S_DESPAWN_USER m)
        {
            if (!TryGetUser(m.EntityId, out var u)) return;
            u.InRange = false;
        }
        private void OnResultItemBidding(S_RESULT_ITEM_BIDDING m)
        {
            EndRoll();
        }
        private void OnResultBiddingDiceThrow(S_RESULT_BIDDING_DICE_THROW m)
        {
            if (!Rolling) StartRoll();
            SetRoll(m.EntityId, m.RollResult);
        }
        private void OnAskBiddingRareItem(S_ASK_BIDDING_RARE_ITEM m)
        {
            StartRoll();
        }
        private void OnLoadTopo(S_LOAD_TOPO m)
        {
            ClearAllAbnormalities();
            SetAggro(0);
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            ClearAll();
        }
        private void OnLogin(S_LOGIN m)
        {
            ClearAll();
        }
        private void OnBanParty(S_BAN_PARTY m)
        {
            ClearAll();
        }
        private void OnLeaveParty(S_LEAVE_PARTY m)
        {
            ClearAll();
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            ClearAll();
        }
        private void OnUserEffect(S_USER_EFFECT m)
        {
            SetAggroCircle(m.Circle, m.Action, m.User);
        }
        private void OnPartyMemberIntervalPosUpdate(S_PARTY_MEMBER_INTERVAL_POS_UPDATE p)
        {
            UpdateMemberLocation(p.PlayerId, p.ServerId, p.Channel, p.ContinentId);
        }

    }
}
