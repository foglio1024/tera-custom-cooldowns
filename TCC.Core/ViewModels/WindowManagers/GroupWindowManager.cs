using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TCC.Data;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{
    public class GroupWindowManager : DependencyObject
    {
        private static GroupWindowManager _instance;
        public static GroupWindowManager Instance => _instance ?? (_instance = new GroupWindowManager());

        const int GROUP_SIZE_THRESHOLD = 7;

        public bool Raid { get; set; }

        private int _groupSize;
        public int GroupSize
        {
            get { return _groupSize; }
            set
            {
                if (_groupSize == value) return;
                _groupSize = value;

                if (_groupSize > GROUP_SIZE_THRESHOLD)
                {
                    WindowManager.GroupWindow.SwitchTemplate(true);
                }
                else
                {
                    WindowManager.GroupWindow.SwitchTemplate(false);
                }
            }
        }
        public bool GetUser(ulong id, out User user)
        {
            User u;
            if (TryGetUserFromList(Dps, id, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Tanks, id, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Healers, id, out u))
            {
                user = u;
                return true;
            }
            user = null;
            return false;

        }
        public bool GetUser(uint playerId, uint serverId, out User user)
        {
            User u;
            if (TryGetUserFromList(Dps, serverId, playerId, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Tanks, serverId, playerId, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Healers, serverId, playerId, out u))
            {
                user = u;
                return true;
            }
            user = null;
            return false;

        }
        public bool GetUser(string name, out User user)
        {
            User u;
            if (TryGetUserFromList(Dps, name, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Tanks, name, out u))
            {
                user = u;
                return true;
            }
            else if (TryGetUserFromList(Healers, name, out u))
            {
                user = u;
                return true;
            }
            user = null;
            return false;

        }

        public bool UserExists(ulong id)
        {
            return GetUser(id, out User u);
        }
        public bool UserExists(string name)
        {
            return GetUser(name, out User u);
        }

        public void BeginOrRefreshUserAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            var size = GroupSize > GROUP_SIZE_THRESHOLD ? AbnormalityManager.RAID_AB_SIZE : AbnormalityManager.PARTY_AB_SIZE;
            var margin = GroupSize > GROUP_SIZE_THRESHOLD ? AbnormalityManager.RAID_AB_LEFT_MARGIN : AbnormalityManager.PARTY_AB_LEFT_MARGIN;

            if (ab.Infinity) duration = uint.MaxValue;
            User u;

            if(GetUser(playerId, serverId, out u))
            {
                if (ab.Type == AbnormalityType.Buff)
                {
                    u.AddOrRefreshBuff(ab, duration, stacks, size, margin);
                    if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203) MoveUser(Dps, Tanks, u.ServerId, u.PlayerId);
                }
                else
                {
                    // -- show only aggro stacks if we are in HH -- //
                    if(BossGageWindowManager.Instance.CurrentNPCs.Any(x => x.ZoneId == 950))
                    {
                        if (ab.Id != 950023) return;
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

            if(GetUser(playerId, serverId, out u))
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

            if(GetUser(playerId, serverId, out u))
            {
                u.ClearAbnormalities();
                return;
            }
        }

        private SynchronizedObservableCollection<User> _healers;
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
        private SynchronizedObservableCollection<User> _tanks;
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
        private SynchronizedObservableCollection<User> _dps;
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
                    if (Tanks.Contains(p))
                    {
                        AddOrUpdateTank(p);
                    }
                    else
                    {
                        AddOrUpdateDps(p);
                    }
                    break;
                default:
                    AddOrUpdateDps(p);
                    break;
            }
            GroupSize = GetCount();

        }
        private void AddOrUpdateDps(User p)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (dps == null)
            {
                _dps.Add(p);
                return;
            }
            dps.Online = p.Online;
            dps.EntityId = p.EntityId;
            dps.IsLeader = p.IsLeader;
            //update here
        }
        private void AddOrUpdateTank(User p)
        {
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (tank == null)
            {
                _tanks.Add(p);
                return;
            }
            tank.Online = p.Online;
            tank.EntityId = p.EntityId;
            tank.IsLeader = p.IsLeader;

            //update here
        }
        private void AddOrUpdateHealer(User p)
        {
            var healer = _healers.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (healer == null)
            {
                _healers.Add(p);
                return;
            }
            healer.Online = p.Online;
            healer.EntityId = p.EntityId;
            healer.IsLeader = p.IsLeader;


            //update here
        }
        public void RemoveMember(uint playerId, uint serverId)
        {
            User u;
            if (TryGetUserFromList(_dps, serverId, playerId, out u))
            {
                Dps.Remove(u);
                GroupSize = GetCount();
                return;
            }
            if (TryGetUserFromList(_tanks, serverId, playerId, out u))
            {
                Tanks.Remove(u);
                GroupSize = GetCount();
            }
            if (TryGetUserFromList(_healers, serverId, playerId, out u))
            {
                Healers.Remove(u);
                GroupSize = GetCount();
            }

        }
        public void ClearAll()
        {
            _healers.Clear();
            _dps.Clear();
            _tanks.Clear();

            Raid = false;
            GroupSize = GetCount();
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
        public void EndRoll()
        {
            //Task.Delay(2000).ContinueWith(t =>
            //{
            foreach (var user in _dps)
            {
                user.IsRolling = false;
                user.IsWinning = false;
                user.RollResult = 0;
            }
            foreach (var user in _tanks)
            {
                user.IsRolling = false;
                user.IsWinning = false;
                user.RollResult = 0;
            }
            foreach (var user in _healers)
            {
                user.IsRolling = false;
                user.IsWinning = false;
                user.RollResult = 0;
            }
            //});
        }
        public void SetRoll(ulong entityId, int rollResult)
        {
            if (rollResult == Int32.MaxValue) rollResult = -1;
            User user;
            if (TryGetUserFromList(_dps, entityId, out user))
            {
                user.RollResult = rollResult;
                FindHighestRoll();
                return;
            }
            if (TryGetUserFromList(_tanks, entityId, out user))
            {
                user.RollResult = rollResult;
                FindHighestRoll();

                return;
            }
            if (TryGetUserFromList(_healers, entityId, out user))
            {
                user.RollResult = rollResult;
                FindHighestRoll();

                return;
            }

        }
        public void StartRoll()
        {
            foreach (var user in _dps)
            {
                user.IsRolling = true;
            }
            foreach (var user in _tanks)
            {
                user.IsRolling = true;
            }
            foreach (var user in _healers)
            {
                user.IsRolling = true;
            }
        }
        private void FindHighestRoll()
        {
            User winningUser = new User(this.Dispatcher) { RollResult = 0 };
            foreach (var user in _dps)
            {
                if (user.RollResult > winningUser.RollResult) winningUser = user;
            }
            foreach (var user in _tanks)
            {
                if (user.RollResult > winningUser.RollResult) winningUser = user;
            }
            foreach (var user in _healers)
            {
                if (user.RollResult > winningUser.RollResult) winningUser = user;
            }
            User u;
            if (TryGetUserFromList(_dps, winningUser.EntityId, out u)) u.IsWinning = true;
            if (TryGetUserFromList(_tanks, winningUser.EntityId, out u)) u.IsWinning = true;
            if (TryGetUserFromList(_healers, winningUser.EntityId, out u)) u.IsWinning = true;
            foreach (var user in _dps)
            {
                if (user.EntityId != winningUser.EntityId) user.IsWinning = false;
            }
            foreach (var user in _tanks)
            {
                if (user.EntityId != winningUser.EntityId) user.IsWinning = false;
            }
            foreach (var user in _healers)
            {
                if (user.EntityId != winningUser.EntityId) user.IsWinning = false;
            }

        }
        private bool TryGetUserFromList(SynchronizedObservableCollection<User> userList, ulong entityId, out User u)
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
        private bool TryGetUserFromList(SynchronizedObservableCollection<User> userList, uint serverId, uint playerId, out User u)
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
        private bool TryGetUserFromList(SynchronizedObservableCollection<User> userList, string name, out User u)
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

        public void SetReadyStatus(ReadyPartyMembers p)
        {
            User user;
            if (TryGetUserFromList(_dps, p.ServerId, p.PlayerId, out user))
            {
                if (p.Status == 0) user.Ready = ReadyStatus.NotReady;
                else if (p.Status == 1) user.Ready = ReadyStatus.Ready;
                return;
            }
            if (TryGetUserFromList(_healers, p.ServerId, p.PlayerId, out user))
            {
                if (p.Status == 0) user.Ready = ReadyStatus.NotReady;
                else if (p.Status == 1) user.Ready = ReadyStatus.Ready;
                return;
            }
            if (TryGetUserFromList(_tanks, p.ServerId, p.PlayerId, out user))
            {
                if (p.Status == 0) user.Ready = ReadyStatus.NotReady;
                else if (p.Status == 1) user.Ready = ReadyStatus.Ready;
                return;
            }
        }
        public void EndReadyCheck()
        {
            Task.Delay(4000).ContinueWith(t =>
            {
                foreach (var user in _dps)
                {
                    user.Ready = ReadyStatus.Undefined;
                }
                foreach (var user in _tanks)
                {
                    user.Ready = ReadyStatus.Undefined;
                }
                foreach (var user in _healers)
                {
                    user.Ready = ReadyStatus.Undefined;
                }
            });
        }
        //use TryGetUser
        public void UpdateMemberHP(uint playerId, uint serverId, int curHP, int maxHP)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (dps != null)
            {
                dps.CurrentHP = curHP;
                dps.MaxHP = maxHP;
                return;
            }
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (tank != null)
            {
                tank.CurrentHP = curHP;
                tank.MaxHP = maxHP;
                return;
            }
            var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (healer != null)
            {
                healer.CurrentHP = curHP;
                healer.MaxHP = maxHP;
                return;
            }

        }
        public void UpdateMemberMP(uint playerId, uint serverId, int curMP, int maxMP)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (dps != null)
            {
                dps.CurrentMP = curMP;
                dps.MaxMP = maxMP;
                return;
            }
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (tank != null)
            {
                tank.CurrentMP = curMP;
                tank.MaxMP = maxMP;
                return;
            }
            var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (healer != null)
            {
                healer.CurrentMP = curMP;
                healer.MaxMP = maxMP;
                return;
            }

        }
        internal void SetRaid(bool raid)
        {
            Dispatcher.Invoke(() => Raid = raid);
        }
        public void UpdateMember(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (dps != null)
            {
                dps.CurrentHP = p.CurrentHP;
                dps.CurrentMP = p.CurrentMP;
                dps.MaxHP = p.MaxHP;
                dps.MaxMP = p.MaxMP;
                dps.Level = (uint)p.Level;
                //dps.Combat = p.Combat;
                dps.Alive = p.Alive;
                //dps.Online = true;
                return;
            }
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (tank != null)
            {
                tank.CurrentHP = p.CurrentHP;
                tank.CurrentMP = p.CurrentMP;
                tank.MaxHP = p.MaxHP;
                tank.MaxMP = p.MaxMP;
                tank.Level = (uint)p.Level;
                //tank.Combat = p.Combat;
                tank.Alive = p.Alive;
                //tank.Online = true;
                return;
            }
            var healer = _healers.FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (healer != null)
            {
                healer.CurrentHP = p.CurrentHP;
                healer.CurrentMP = p.CurrentMP;
                healer.MaxHP = p.MaxHP;
                healer.MaxMP = p.MaxMP;
                healer.Level = (uint)p.Level;
                //healer.Combat = p.Combat;
                healer.Alive = p.Alive;
                //healer.Online = true;
                return;
            }
        }
        private int GetCount()
        {
            var c = _healers.Count + _dps.Count + _tanks.Count;
            if (SettingsManager.IgnoreMeInGroupWindow) c++;
            return c;
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
            if(GetUser(name, out User u))
            {
                if (u.IsLeader) return true;
            }
            return false;
        }
    }
}
