using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{
    public class GroupWindowViewModel : TSPropertyChanged
    {
        private static GroupWindowViewModel _instance;
        public static GroupWindowViewModel Instance => _instance ?? (_instance = new GroupWindowViewModel());

        public const int GROUP_SIZE_THRESHOLD = 7;
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale == value) return;
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }
        private double scale = SettingsManager.GroupWindowSettings.Scale;
        public GroupWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.GroupWindow.RefreshTopmost();
                }
            };

            Dps = new SynchronizedObservableCollection<Data.User>(_dispatcher);
            Healers = new SynchronizedObservableCollection<Data.User>(_dispatcher);
            Tanks = new SynchronizedObservableCollection<Data.User>(_dispatcher);

            _dps.CollectionChanged += _dps_CollectionChanged;
            _healers.CollectionChanged += _healers_CollectionChanged;
            _tanks.CollectionChanged += _tanks_CollectionChanged;
        }
        private void _tanks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TanksCount));
            Task.Delay(100).ContinueWith(t => NotifyPropertyChanged(nameof(GroupSize)));
            NotifyPropertyChanged(nameof(Formed));
            NotifyPropertyChanged(nameof(AliveMembersCount));
            NotifyPropertyChanged(nameof(ReadyMembersCount));
        }
        private void _healers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(HealersCount));
            Task.Delay(100).ContinueWith(t => NotifyPropertyChanged(nameof(GroupSize)));
            NotifyPropertyChanged(nameof(Formed));
            NotifyPropertyChanged(nameof(AliveMembersCount));
            NotifyPropertyChanged(nameof(ReadyMembersCount));

        }
        private void _dps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(DpsCount));
            Task.Delay(100).ContinueWith(t => NotifyPropertyChanged(nameof(GroupSize)));
            NotifyPropertyChanged(nameof(Formed));
            NotifyPropertyChanged(nameof(AliveMembersCount));
            NotifyPropertyChanged(nameof(ReadyMembersCount));

        }
        public bool Raid
        {
            get => raid;
            set
            {
                if (raid == value) return;
                raid = value;
                NotifyPropertyChanged(nameof(Raid));
            }
        }
        private bool raid;
        public int GroupSize
        {
            get { return All.Count; }
        }
        public int ReadyMembersCount
        {
            get => All.Where(x => x.Ready == ReadyStatus.Ready).Count();
        }
        public int AliveMembersCount
        {
            get => All.Where(x => x.Alive).Count();
        }
        public int DpsCount
        {
            get { return _dps.Count; }
        }
        public int HealersCount
        {
            get { return _healers.Count; }
        }
        public int TanksCount
        {
            get { return _tanks.Count; }
        }
        public bool Formed
        {
            get => GroupSize > 0 ? true : false;
        }
        public bool MPenabled
        {
            get => mpEnabled;
            set
            {
                if (mpEnabled == value) return;
                mpEnabled = value;
                NotifyPropertyChanged(nameof(MPenabled));
            }
        }
        private bool mpEnabled;
        public bool HPenabled
        {
            get => hpEnabled;
            set
            {
                if (hpEnabled == value) return;
                hpEnabled = value;
                NotifyPropertyChanged(nameof(HPenabled));
            }
        }
        private bool hpEnabled;

        public List<User> All
        {
            get
            {
                var l = new List<User>();
                l.AddRange(_tanks.ToList());
                l.AddRange(_healers.ToList());
                l.AddRange(_dps.ToList());
                return l;
            }
        }
        public bool GetUser(ulong id, out User user)
        {
            User u;
            if (TryGetUserFromList(All, id, out u))
            {
                user = u;
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }
        public bool GetUser(uint playerId, uint serverId, out User user)
        {
            User u;
            if (TryGetUserFromList(All, serverId, playerId, out u))
            {
                user = u;
                return true;
            }
            else
            {
                user = null;
                return false;
            }

        }
        public bool GetUser(string name, out User user)
        {
            User u;
            if (TryGetUserFromList(All, name, out u))
            {
                user = u;
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        public bool HasPowers(string name)
        {
            GetUser(name, out User u);
            return u != null ? u.CanInvite : false;
        }


        public bool UserExists(ulong id)
        {
            return GetUser(id, out User u);
        }
        public bool UserExists(string name)
        {
            return GetUser(name, out User u);
        }
        public bool AmILeader()
        {
            return IsLeader(SessionManager.CurrentPlayer.Name);
        }

        public void SetAggro(S_NPC_STATUS p)
        {
            if (p.Target == 0) { ResetAggro(); return; }
            if (aggroHolder == p.Target) return;
            else
            {
                User u;
                if (GetUser(p.Target, out u))
                {
                    foreach (var item in All)
                    {
                        if (item == u)
                        {
                            item.HasAggro = true;
                            aggroHolder = item.EntityId;
                        }
                        else item.HasAggro = false;
                    }
                }
            }
        }
        public void ResetAggro()
        {
            foreach (var item in All)
            {
                item.HasAggro = false;
            }
        }
        public void BeginOrRefreshUserAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            var size = GroupSize > GROUP_SIZE_THRESHOLD ? AbnormalityManager.RAID_AB_SIZE : AbnormalityManager.PARTY_AB_SIZE;
            var margin = GroupSize > GROUP_SIZE_THRESHOLD ? AbnormalityManager.RAID_AB_LEFT_MARGIN : AbnormalityManager.PARTY_AB_LEFT_MARGIN;

            if (ab.Infinity) duration = uint.MaxValue;
            User u;

            if (GetUser(playerId, serverId, out u))
            {
                if (ab.Type == AbnormalityType.Buff)
                {
                    u.AddOrRefreshBuff(ab, duration, stacks, size, margin);
                    if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203) MoveUser(Dps, Tanks, u.ServerId, u.PlayerId);
                }
                else
                {
                    // -- show only aggro stacks if we are in HH -- //
                    if (BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase2 ||
                        BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase3 ||
                        BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase4)
                    {
                        if (ab.Id != 950023 && SettingsManager.ShowOnlyAggroStacks) return;
                    }
                    // -------------------------------------------- //
                    u.AddOrRefreshDebuff(ab, duration, stacks, size, margin);
                }
                return;
            }
        }
        public void EndUserAbnormality(Abnormality ab, uint playerId, uint serverId)
        {
            User u;

            if (GetUser(playerId, serverId, out u))
            {
                if (ab.Type == AbnormalityType.Buff)
                {
                    u.RemoveBuff(ab);
                    if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203) MoveUser(Tanks, Dps, u.ServerId, u.PlayerId);
                }
                else
                {
                    u.RemoveDebuff(ab);
                }
                return;

            }
        }
        public void ClearUserAbnormality(uint playerId, uint serverId)
        {
            User u;

            if (GetUser(playerId, serverId, out u))
            {
                u.ClearAbnormalities();
                return;
            }
        }

        ulong aggroHolder;
        public void SetAggro(S_USER_EFFECT p)
        {
            if (BossGageWindowViewModel.Instance.CurrentHHphase != HarrowholdPhase.None) return;
            User u;
            if (GetUser(p.User, out u))
            {
                if (p.Circle == AggroCircle.Main)
                {
                    if (p.Action == AggroAction.Add)
                    {
                        aggroHolder = p.User;
                        u.HasAggro = true;
                    }
                    else
                    {
                        u.HasAggro = false;
                    }
                }
            }
        }


        public SynchronizedObservableCollection<User> Healers
        {
            get
            {
                return _healers;
            }
            set
            {
                if (_healers == value) return;
                _healers = value;
            }
        }
        private SynchronizedObservableCollection<User> _healers;
        public SynchronizedObservableCollection<User> Tanks
        {
            get
            {
                return _tanks;
            }
            set
            {
                if (_tanks == value) return;
                _tanks = value;
            }
        }
        private SynchronizedObservableCollection<User> _tanks;
        public SynchronizedObservableCollection<User> Dps
        {
            get
            {
                return _dps;
            }
            set
            {
                if (_dps == value) return;
                _dps = value;
            }
        }
        private SynchronizedObservableCollection<User> _dps;

        private readonly object dpsLock = new object();
        private readonly object tankLock = new object();
        private readonly object healersLock = new object();
        private readonly object genericLock = new object();

        public void AddOrUpdateMember(User p)
        {
            if (SettingsManager.IgnoreMeInGroupWindow && p.EntityId == SessionManager.CurrentPlayer.EntityId) return;
            switch (p.UserClass)
            {
                case Class.Priest:
                    AddOrUpdateHealer(p);
                    break;
                case Class.Elementalist:
                    AddOrUpdateHealer(p);
                    break;
                case Class.Lancer:
                    AddOrUpdateTank(p);
                    break;
                case Class.Fighter:
                    AddOrUpdateTank(p);
                    break;
                case Class.Warrior:
                    AddOrUpdateWarrior(p);
                    break;
                default:
                    AddOrUpdateDps(p);
                    break;
            }

        }
        private void AddOrUpdateWarrior(User p)
        {
            if (TryGetUserFromList(Tanks, p.ServerId, p.PlayerId, out User wartank))
            {
                AddOrUpdateTank(p);
            }
            else if (TryGetUserFromList(Dps, p.ServerId, p.PlayerId, out User warrior))
            {
                AddOrUpdateDps(p);
            }
            else
            {
                AddOrUpdateDps(p);
            }

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
            SystemMessages.Messages.TryGetValue(opcode, out SystemMessage m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
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
            SystemMessages.Messages.TryGetValue(opcode, out SystemMessage m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);

        }
        private void AddOrUpdateDps(User p)
        {
            lock (dpsLock)
            {
                var dps = _dps.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (dps == null)
                {
                    _dps.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }
                dps.Online = p.Online;
                dps.EntityId = p.EntityId;
                dps.IsLeader = p.IsLeader;
                //update here
            }
        }
        private void AddOrUpdateTank(User p)
        {
            lock (tankLock)
            {
                var tank = _tanks.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (tank == null)
                {
                    _tanks.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }
                tank.Online = p.Online;
                tank.EntityId = p.EntityId;
                tank.IsLeader = p.IsLeader;

            }
            //update here
        }
        private void AddOrUpdateHealer(User p)
        {
            lock (healersLock)
            {
                var healer = _healers.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (healer == null)
                {
                    _healers.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }
                healer.Online = p.Online;
                healer.EntityId = p.EntityId;
                healer.IsLeader = p.IsLeader;

                //update here
            }
        }
        public void RemoveMember(uint playerId, uint serverId, bool kick = false)
        {
            User u;
            if (TryGetUserFromList(_dps, serverId, playerId, out u))
            {
                Dps.Remove(u);
                if (!kick) SendLeaveMessage(u.Name);
                return; //needed?
            }
            if (TryGetUserFromList(_tanks, serverId, playerId, out u))
            {
                Tanks.Remove(u);
                if (!kick) SendLeaveMessage(u.Name);
            }
            if (TryGetUserFromList(_healers, serverId, playerId, out u))
            {
                Healers.Remove(u);
                if (!kick) SendLeaveMessage(u.Name);
            }

        }
        public void ClearAll()
        {
            _healers.Clear();
            _dps.Clear();
            _tanks.Clear();

            Raid = false;
        }
        public void LogoutMember(uint playerId, uint serverId)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (dps != null)
            {
                dps.Online = false;
                return;
            }
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (tank != null)
            {
                tank.Online = false;
                return;
            }
            var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (healer != null)
            {
                healer.Online = false;
                return;
            }
        }
        internal void RemoveMe()
        {
            User u;
            if (TryGetUserFromList(_dps, SessionManager.CurrentPlayer.EntityId, out u)) { _dps.Remove(u); return; }
            if (TryGetUserFromList(_tanks, SessionManager.CurrentPlayer.EntityId, out u)) { _tanks.Remove(u); return; }
            if (TryGetUserFromList(_healers, SessionManager.CurrentPlayer.EntityId, out u)) { _healers.Remove(u); return; }
        }
        internal void ClearMyBuffs()
        {
            User u;
            if (TryGetUserFromList(_dps, SessionManager.CurrentPlayer.EntityId, out u)) { u.Buffs.Clear(); return; }
            if (TryGetUserFromList(_tanks, SessionManager.CurrentPlayer.EntityId, out u)) { u.Buffs.Clear(); return; }
            if (TryGetUserFromList(_healers, SessionManager.CurrentPlayer.EntityId, out u)) { u.Buffs.Clear(); return; }
        }
        internal void SetNewLeader(ulong entityId, string name)
        {
            foreach (var u in _dps)
            {
                if (u.Name == name) u.IsLeader = true;
                else u.IsLeader = false;
            }
            foreach (var u in _tanks)
            {
                if (u.Name == name) u.IsLeader = true;
                else u.IsLeader = false;
            }
            foreach (var u in _healers)
            {
                if (u.Name == name) u.IsLeader = true;
                else u.IsLeader = false;
            }
        }
        internal void ClearAllBuffs()
        {
            foreach (var user in _dps)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
            }
            foreach (var user in _tanks)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
            }
            foreach (var user in _healers)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
            }

        }
        internal void ClearAllAbnormalities()
        {
            foreach (var user in _dps)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
                foreach (var b in user.Debuffs)
                {
                    b.Dispose();
                }
                user.Debuffs.Clear();

            }
            foreach (var user in _tanks)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
                foreach (var b in user.Debuffs)
                {
                    b.Dispose();
                }
                user.Debuffs.Clear();

            }
            foreach (var user in _healers)
            {
                foreach (var b in user.Buffs)
                {
                    b.Dispose();
                }
                user.Buffs.Clear();
                foreach (var b in user.Debuffs)
                {
                    b.Dispose();
                }
                user.Debuffs.Clear();

            }
        }
        public void StartRoll()
        {
            foreach (var user in All)
            {
                user.IsRolling = true;
            }
        }
        public void SetRoll(ulong entityId, int rollResult)
        {
            if (rollResult == Int32.MaxValue) rollResult = -1;
            if (TryGetUserFromList(All, entityId, out User user))
            {
                user.RollResult = rollResult;
                FindHighestRoll();
            }
        }
        public void EndRoll()
        {
            foreach (var user in All)
            {
                user.IsRolling = false;
                user.IsWinning = false;
                user.RollResult = 0;
            }
        }
        private void FindHighestRoll()
        {
            User winningUser = new User(_dispatcher) { RollResult = 0 };
            foreach (var user in All)
            {
                if (user.RollResult > winningUser.RollResult) winningUser = user;
            }
            User u;
            if (TryGetUserFromList(All, winningUser.EntityId, out u)) u.IsWinning = true;
            foreach (var user in All)
            {
                if (user.EntityId != winningUser.EntityId) user.IsWinning = false;
            }
        }
        private bool TryGetUserFromList(IEnumerable<User> userList, ulong entityId, out User u)
        {
            lock (genericLock)
            {
                u = userList.FirstOrDefault(x => x.EntityId == entityId);
                if (u != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private bool TryGetUserFromList(IEnumerable<User> userList, uint serverId, uint playerId, out User u)
        {
            lock (genericLock)
            {
                u = userList.FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
                if (u != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private bool TryGetUserFromList(IEnumerable<User> userList, string name, out User u)
        {
            lock (genericLock)
            {
                u = userList.FirstOrDefault(x => x.Name == name);
                if (u != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        bool _firstCheck = true;
        public void SetReadyStatus(ReadyPartyMember p)
        {
            if (_firstCheck)
            {
                foreach (var usr in All)
                {
                    usr.Ready = ReadyStatus.Undefined;
                }
            }
            var user = All.FirstOrDefault(u => u.PlayerId == p.PlayerId && u.ServerId == p.ServerId);
            if (user != null)
            {
                if (p.Status == 0) user.Ready = ReadyStatus.NotReady;
                else if (p.Status == 1) user.Ready = ReadyStatus.Ready;
            }
            _firstCheck = false;
            NotifyPropertyChanged(nameof(ReadyMembersCount));
        }
        public void EndReadyCheck()
        {
            Task.Delay(4000).ContinueWith(t =>
            {
                foreach (var user in All)
                {
                    user.Ready = ReadyStatus.None;
                }
            });
            _firstCheck = true;
        }
        //use TryGetUser
        public void UpdateMemberHP(uint playerId, uint serverId, int curHP, int maxHP)
        {
            var u = All.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u != null)
            {
                u.CurrentHP = curHP;
                u.MaxHP = maxHP;
            }
            //var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (dps != null)
            //{
            //    dps.CurrentHP = curHP;
            //    dps.MaxHP = maxHP;
            //    return;
            //}
            //var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (tank != null)
            //{
            //    tank.CurrentHP = curHP;
            //    tank.MaxHP = maxHP;
            //    return;
            //}
            //var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (healer != null)
            //{
            //    healer.CurrentHP = curHP;
            //    healer.MaxHP = maxHP;
            //    return;
            //}

        }
        public void UpdateMemberMP(uint playerId, uint serverId, int curMP, int maxMP)
        {
            var u = All.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u != null)
            {
                u.CurrentMP = curMP;
                u.MaxMP = maxMP;
            }
            //var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (dps != null)
            //{
            //    dps.CurrentMP = curMP;
            //    dps.MaxMP = maxMP;
            //    return;
            //}
            //var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (tank != null)
            //{
            //    tank.CurrentMP = curMP;
            //    tank.MaxMP = maxMP;
            //    return;
            //}
            //var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            //if (healer != null)
            //{
            //    healer.CurrentMP = curMP;
            //    healer.MaxMP = maxMP;
            //    return;
            //}

        }
        internal void SetRaid(bool raid)
        {
            _dispatcher.Invoke(() => Raid = raid);
        }
        public void UpdateMember(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            var u = All.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (u != null)
            {
                u.CurrentHP = p.CurrentHP;
                u.CurrentMP = p.CurrentMP;
                u.MaxHP = p.MaxHP;
                u.MaxMP = p.MaxMP;
                u.Level = (uint)p.Level;
                if (u.Alive != p.Alive) NotifyPropertyChanged(nameof(AliveMembersCount));
                u.Alive = p.Alive;
                if (!p.Alive) u.HasAggro = false;
            }

            //var dps = _dps.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            //if (dps != null)
            //{
            //    dps.CurrentHP = p.CurrentHP;
            //    dps.CurrentMP = p.CurrentMP;
            //    dps.MaxHP = p.MaxHP;
            //    dps.MaxMP = p.MaxMP;
            //    dps.Level = (uint)p.Level;
            //    //dps.Combat = p.Combat;
            //    dps.Alive = p.Alive;
            //    //dps.Online = true;
            //    return;
            //}
            //var tank = _tanks.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            //if (tank != null)
            //{
            //    tank.CurrentHP = p.CurrentHP;
            //    tank.CurrentMP = p.CurrentMP;
            //    tank.MaxHP = p.MaxHP;
            //    tank.MaxMP = p.MaxMP;
            //    tank.Level = (uint)p.Level;
            //    //tank.Combat = p.Combat;
            //    tank.Alive = p.Alive;
            //    //tank.Online = true;
            //    return;
            //}
            //var healer = _healers.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            //if (healer != null)
            //{
            //    healer.CurrentHP = p.CurrentHP;
            //    healer.CurrentMP = p.CurrentMP;
            //    healer.MaxHP = p.MaxHP;
            //    healer.MaxMP = p.MaxMP;
            //    healer.Level = (uint)p.Level;
            //    //healer.Combat = p.Combat;
            //    healer.Alive = p.Alive;
            //    //healer.Online = true;
            //    return;
            //}
        }
        public void MoveUser(SynchronizedObservableCollection<User> startList, SynchronizedObservableCollection<User> endList, uint serverId, uint playerId)
        {
            if (TryGetUserFromList(startList, serverId, playerId, out User u))
            {
                if (TryGetUserFromList(endList, serverId, playerId, out User x)) return;
                endList.Add(u);
                startList.Remove(u);
            }
        }
        public bool IsLeader(string name)
        {
            if (GetUser(name, out User u))
            {
                if (u.IsLeader) return true;
            }
            return false;
        }
    }
}
