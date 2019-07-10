using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FoglioUtils;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Settings;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{

    [TccModule]
    public class GroupWindowViewModel : TccWindowViewModel
    {
        private bool _raid;
        private bool _firstCheck = true;
        private readonly object _lock = new object();
        private bool _leaderOverride;
        private ulong _aggroHolder;
        public event Action SettingsUpdated;

        public SynchronizedObservableCollection<User> Members { get; }
        public GroupWindowLayout GroupWindowLayout => App.Settings.GroupWindowSettings.Layout;

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
        public bool ShowDetails => Formed && App.Settings.GroupWindowSettings.ShowDetails;
        public bool ShowLeaveButton => Formed && /*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable;
        public bool ShowLeaderButtons => Formed && /*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable && AmILeader;
        public bool Rolling { get; set; }

        public GroupWindowViewModel(WindowSettings settings) : base(settings)
        {
            Members = new SynchronizedObservableCollection<User>(Dispatcher);
            Members.CollectionChanged += Members_CollectionChanged;

            Dps = CollectionViewUtils.InitLiveView(o => ((User)o).Role == Role.Dps && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Tanks = CollectionViewUtils.InitLiveView(o => ((User)o).Role == Role.Tank && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Healers = CollectionViewUtils.InitLiveView(o => ((User)o).Role == Role.Healer && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            All = CollectionViewUtils.InitLiveView(o => ((User)o).Visible, Members, new[] { nameof(User.Visible) }, new[]
            {
                new SortDescription(nameof(User.Role), ListSortDirection.Descending),
                new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)
            });

            ((ICollectionView)Dps).CollectionChanged += GcPls;
            ((ICollectionView)Tanks).CollectionChanged += GcPls;
            ((ICollectionView)Healers).CollectionChanged += GcPls;
            ((ICollectionView)All).CollectionChanged += GcPls;

            Session.Teleported += OnTeleported;
        }

        private void OnTeleported()
        {
            if (!Session.CivilUnrestZone)
                PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
            else
                PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(OnPartyMemberIntervalPosUpdate);
        }

        private void GcPls(object sender, EventArgs ev) { }

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
        public void NotifySettingUpdated()
        {
            SettingsUpdated?.Invoke();

            N(nameof(ShowDetails));
        }
        public bool Exists(ulong id)
        {
            return Members.ToSyncList().Any(x => x.EntityId == id);
        }
        public bool Exists(string name)
        {
            return Members.ToSyncList().Any(x => x.Name == name);
        }
        public bool Exists(uint pId, uint sId)
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
        public bool HasPowers(string name)
        {
            return Members.ToSyncList().FirstOrDefault(x => x.Name == name)?.CanInvite ?? false;
        }
        public bool AmILeader => IsLeader(Session.Me.Name) || _leaderOverride;

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
        public void SetAggroCircle(AggroCircle circle, AggroAction action, ulong user)
        {
            if (WindowManager.ViewModels.NPC.CurrentHHphase != HarrowholdPhase.None) return;

            if (circle != AggroCircle.Main) return;
            if (action == AggroAction.Add)
            {
                SetAggro(user);
            }
        }
        public void BeginOrRefreshAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ab.Infinity) duration = uint.MaxValue;
                var u = Members.ToSyncList().FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
                if (u == null) return;

                if (ab.Type == AbnormalityType.Buff)
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
                    if (WindowManager.ViewModels.NPC.CurrentHHphase >= HarrowholdPhase.Phase2)
                    {
                        if (ab.Id != 950023 && App.Settings.GroupWindowSettings.ShowOnlyAggroStacks) return;
                    }
                    // -------------------------------------------- //
                    u.AddOrRefreshDebuff(ab, duration, stacks);
                }
            }));
        }
        public void EndAbnormality(Abnormality ab, uint playerId, uint serverId)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;

                if (ab.Type == AbnormalityType.Buff)
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
            }));
        }
        public void ClearAbnormality(uint playerId, uint serverId)
        {
            Dispatcher.Invoke(() =>
            {
                Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId)?.ClearAbnormalities();
            });
        }
        public void AddOrUpdateMember(User p)
        {
            if (App.Settings.GroupWindowSettings.IgnoreMe && p.IsPlayer)
            {
                _leaderOverride = p.IsLeader;
                p.Visible = false;
                //return;
            }
            lock (_lock) //TODO: really needed?
            {
                var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (user == null)
                {
                    Members.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }

                if (user.Online != p.Online) SendOnlineMessage(user.Name, p.Online);
                user.Online = p.Online;
                user.EntityId = p.EntityId;
                user.IsLeader = p.IsLeader;
                user.Order = p.Order;
                user.Awakened = p.Awakened;
                user.Visible = p.Visible;
            }
        }
        public void AddOrUpdateMember(PartyMemberData p)
        {
            var visible = true;
            if (App.Settings.GroupWindowSettings.IgnoreMe && p.Name == Session.Me.Name)
            {
                _leaderOverride = p.IsLeader;
                visible = false;
                //return;
            }
            lock (_lock) //TODO: really needed?
            {
                var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (user == null)
                {
                    Members.Add(new User(p));
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
            }
        }

        private void SendOnlineMessage(string name, bool newVal)
        {
            var opcode = newVal ? "TCC_PARTY_MEMBER_LOGON" : "TCC_PARTY_MEMBER_LOGOUT";
            var msg = "@0\vUserName\v" + name;
            Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
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
            Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
        }
        private void SendDeathMessage(string name)
        {
            string msg;
            string opcode;
            if (Raid)
            {
                msg = "@0\vPartyPlayerName\v" + name;
                opcode = "SMT_BATTLE_PARTY_DIE";
            }
            else
            {
                opcode = "SMT_BATTLE_PARTY_DIE";
                msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
            }
            Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
            if (/*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable) ProxyInterface.Instance.Stub.ForceSystemMessage(msg, opcode); //ProxyOld.ForceSystemMessage(msg, opcode);
        }
        private void SendLeaveMessage(string name)
        {
            string msg;
            string opcode;
            if (Raid)
            {
                msg = "@0\vPartyPlayerName\v" + name;
                opcode = "SMT_LEAVE_UNIONPARTY_PARTYPLAYER";
            }
            else
            {
                opcode = "SMT_LEAVE_PARTY_PARTYPLAYER";
                msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
            }
            Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);

        }
        public void RemoveMember(uint playerId, uint serverId, bool kick = false)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.ClearAbnormalities();
            Members.Remove(u);
            if (!kick) SendLeaveMessage(u.Name);
        }
        public void ClearAll()
        {
            if (!App.Settings.GroupWindowSettings.Enabled || !Dispatcher.Thread.IsAlive) return;
            Members.ToSyncList().ForEach(x => x.ClearAbnormalities());
            Members.Clear();
            Raid = false;
            _leaderOverride = false;
        }
        public void LogoutMember(uint playerId, uint serverId)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.Online = false;
        }
        public void ToggleMe(bool visible)
        {
            var me = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
            if (me == null) return;
            me.Visible = visible;
            //me.ClearAbnormalities();
            //Members.Remove(me);
        }
        internal void ClearAllAbnormalities()
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
        public void SetNewLeader(ulong entityId, string name)
        {
            foreach (var m in Members.ToSyncList())
            {
                m.IsLeader = m.Name == name;
            }
            _leaderOverride = name == Session.Me.Name;
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
        public void SetRoll(ulong entityId, int rollResult)
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
        public void EndRoll()
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
        public void EndReadyCheck()
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
        public void UpdateMemberHp(uint playerId, uint serverId, int curHp, int maxHp)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentHp = curHp;
                u.MaxHp = maxHp;
            }));
        }
        public void UpdateMemberMp(uint playerId, uint serverId, int curMp, int maxMp)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentMp = curMp;
                u.MaxMp = maxMp;
            }));
        }
        public void SetRaid(bool raid)
        {
            Dispatcher.BeginInvoke(new Action(() => Raid = raid));
        }
        public void UpdateMember(PartyMemberData p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (u == null) return;
                u.CurrentHp = p.CurrentHP;
                u.CurrentMp = p.CurrentMP;
                u.MaxHp = p.MaxHP;
                u.MaxMp = p.MaxMP;
                u.Level = p.Level;
                if (u.Alive && !p.Alive) SendDeathMessage(u.Name);
                u.Alive = p.Alive;
                N(nameof(AliveCount));
                if (!p.Alive) u.HasAggro = false;
            }));
        }
        public void NotifyThresholdChanged()
        {
            N(nameof(Size));
        }
        public void UpdateMemberGear(uint playerId, uint serverId, GearItemData weapon, GearItemData armor, GearItemData hands, GearItemData feet)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.Weapon = new GearItem(weapon);
            u.Armor = new GearItem(armor);
            u.Gloves = new GearItem(hands);
            u.Boots = new GearItem(feet);
        }
        public void UpdateMyGear()
        {
            //var u = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
            //if (u == null) return;
            //var currCharGear = WindowManager.ViewModels.Dashboard.CurrentCharacter.Gear;
            //u.Weapon = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Weapon);
            //u.Armor = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Armor);
            //u.Gloves = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Hands);
            //u.Boots = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Feet);

        }
        public void UpdateMemberLocation(uint playerId, uint serverId, int channel, uint continentId)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            var ch = channel > 1000 ? "" : " ch." + channel;
            u.Location = Session.DB.TryGetGuardOrDungeonNameFromContinentId(continentId, out var l) ? l + ch : "Unknown";
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.NewProcessor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.NewProcessor.Hook<S_LEAVE_PARTY>(OnLeaveParty);
            PacketAnalyzer.NewProcessor.Hook<S_BAN_PARTY>(OnBanParty);
            PacketAnalyzer.NewProcessor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Hook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.NewProcessor.Hook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
            PacketAnalyzer.NewProcessor.Hook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
            PacketAnalyzer.NewProcessor.Hook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
            PacketAnalyzer.NewProcessor.Hook<S_SPAWN_USER>(OnSpawnUser);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_BUFF_UPDATE>(OnPartyMemberBuffUpdate);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_ABNORMAL_ADD>(OnPartyMemberAbnormalAdd);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_ABNORMAL_DEL>(OnPartyMemberAbnormalDel);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_ABNORMAL_CLEAR>(OnPartyMemberAbnormalClear);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_ABNORMAL_REFRESH>(OnPartyMemberAbnormalRefresh);
            PacketAnalyzer.NewProcessor.Hook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
            PacketAnalyzer.NewProcessor.Hook<S_LEAVE_PARTY_MEMBER>(OnLeavePartyMember);
            PacketAnalyzer.NewProcessor.Hook<S_BAN_PARTY_MEMBER>(OnBanPartyMember);
            PacketAnalyzer.NewProcessor.Hook<S_LOGOUT_PARTY_MEMBER>(OnLogoutPartyMember);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_CHANGE_HP>(OnPartyMemberChangeHp);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_CHANGE_MP>(OnPartyMemberChangeMp);
            PacketAnalyzer.NewProcessor.Hook<S_PARTY_MEMBER_STAT_UPDATE>(OnPartyMemberStatUpdate);
            PacketAnalyzer.NewProcessor.Hook<S_CHECK_TO_READY_PARTY>(OnCheckToReadyParty);
            PacketAnalyzer.NewProcessor.Hook<S_CHECK_TO_READY_PARTY_FIN>(OnCheckToReadyPartyFin);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.NewProcessor.Unhook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.NewProcessor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.NewProcessor.Unhook<S_LEAVE_PARTY>(OnLeaveParty);
            PacketAnalyzer.NewProcessor.Unhook<S_BAN_PARTY>(OnBanParty);
            PacketAnalyzer.NewProcessor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Unhook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.NewProcessor.Unhook<S_ASK_BIDDING_RARE_ITEM>(OnAskBiddingRareItem);
            PacketAnalyzer.NewProcessor.Unhook<S_RESULT_BIDDING_DICE_THROW>(OnResultBiddingDiceThrow);
            PacketAnalyzer.NewProcessor.Unhook<S_RESULT_ITEM_BIDDING>(OnResultItemBidding);
            PacketAnalyzer.NewProcessor.Unhook<S_SPAWN_USER>(OnSpawnUser);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_BUFF_UPDATE>(OnPartyMemberBuffUpdate);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_ABNORMAL_ADD>(OnPartyMemberAbnormalAdd);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_ABNORMAL_DEL>(OnPartyMemberAbnormalDel);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_ABNORMAL_CLEAR>(OnPartyMemberAbnormalClear);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_ABNORMAL_REFRESH>(OnPartyMemberAbnormalRefresh);
            PacketAnalyzer.NewProcessor.Unhook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
            PacketAnalyzer.NewProcessor.Unhook<S_LEAVE_PARTY_MEMBER>(OnLeavePartyMember);
            PacketAnalyzer.NewProcessor.Unhook<S_BAN_PARTY_MEMBER>(OnBanPartyMember);
            PacketAnalyzer.NewProcessor.Unhook<S_LOGOUT_PARTY_MEMBER>(OnLogoutPartyMember);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_CHANGE_HP>(OnPartyMemberChangeHp);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_CHANGE_MP>(OnPartyMemberChangeMp);
            PacketAnalyzer.NewProcessor.Unhook<S_PARTY_MEMBER_STAT_UPDATE>(OnPartyMemberStatUpdate);
            PacketAnalyzer.NewProcessor.Unhook<S_CHECK_TO_READY_PARTY>(OnCheckToReadyParty);
            PacketAnalyzer.NewProcessor.Unhook<S_CHECK_TO_READY_PARTY_FIN>(OnCheckToReadyPartyFin);
        }

        private void OnCheckToReadyPartyFin(S_CHECK_TO_READY_PARTY_FIN p)
        {
            EndReadyCheck();
        }
        private void OnCheckToReadyParty(S_CHECK_TO_READY_PARTY p)
        {
            Dispatcher.BeginInvoke(new Action(() => p.Party.ForEach(SetReadyStatus)));
        }
        private void OnPartyMemberStatUpdate(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            UpdateMember(p.PartyMemberData);
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
            Dispatcher.BeginInvoke(new Action(() => p.Members.ForEach(AddOrUpdateMember)));
        }
        private void OnChangePartyManager(S_CHANGE_PARTY_MANAGER m)
        {
            SetNewLeader(m.EntityId, m.Name);
        }
        private void OnPartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH m)
        {
            AbnormalityManager.UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
        }
        private void OnPartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR m)
        {
            ClearAbnormality(m.PlayerId, m.ServerId);
        }
        private void OnPartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL m)
        {
            AbnormalityManager.EndPartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id);
        }
        private void OnPartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD m)
        {
            AbnormalityManager.UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, m.Id, m.Duration, m.Stacks);
        }
        private void OnPartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE m)
        {
            foreach (var buff in m.Abnormals) AbnormalityManager.UpdatePartyMemberAbnormality(m.PlayerId, m.ServerId, buff.Id, buff.Duration, buff.Stacks);
        }
        private void OnSpawnUser(S_SPAWN_USER m)
        {
            if (!Exists(m.EntityId)) return;
            UpdateMemberGear(m.PlayerId, m.ServerId, m.Weapon, m.Armor, m.Gloves, m.Boots);
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
