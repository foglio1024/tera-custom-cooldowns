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

        private int _groupSize;
        public int GroupSize
        {
            get { return _groupSize; }
            set
            {
                if (_groupSize == value) return;
                _groupSize = value;

                if (_groupSize > 10)
                {
                    WindowManager.GroupWindow.SwitchTemplate(true);
                }
                else
                {
                    WindowManager.GroupWindow.SwitchTemplate(false);
                }

                Console.WriteLine("Group size: " + _groupSize);
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
                    // add def/assault stance logic
                    AddOrUpdateDps(p);
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
            //update here
        }

        public void RemoveMember(uint playerId, uint serverId)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (dps != null)
            {
                _dps.Remove(dps);
                GroupSize = GetCount();
                return;
            }
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (tank != null)
            {
                _tanks.Remove(tank);
                GroupSize = GetCount();
                return;
            }
            var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (healer != null)
            {
                _healers.Remove(healer);
                GroupSize = GetCount();
                return;
            }

        }
        private void RemoveHealer(uint playerId, uint serverId)
        {
            var healer = _healers.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (healer == null) return;
            _healers.Remove(healer);
        }
        private void RemoveTank(uint playerId, uint serverId)
        {
            var tank = _tanks.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (tank == null) return;
            _tanks.Remove(tank);
        }
        private void RemoveDps(uint playerId, uint serverId)
        {
            var dps = _dps.FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (dps == null) return;
            _dps.Remove(dps);
        }

        public void ClearAll()
        {
            _healers.Clear();
            _dps.Clear();
            _tanks.Clear();

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

        private int GetCount()
        {
            return _healers.Count + _dps.Count + _tanks.Count;
        }

        internal void UpdateMember(S_PARTY_MEMBER_STAT_UPDATE p)
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
    }
}
