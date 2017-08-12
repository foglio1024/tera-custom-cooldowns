using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TCC.ClassSpecific;
using TCC.ViewModels;

namespace TCC.Data
{
    public class User : TSPropertyChanged
    {
        private ulong entityId;
        public ulong EntityId
        {
            get { return entityId; }
            set
            {
                if (entityId == value) return;
                entityId = value;
                NotifyPropertyChanged("EntityId");
            }
        }

        private uint level;
        public uint Level
        {
            get { return level; }
            set
            {
                if (level == value) return;
                level = value;
                NotifyPropertyChanged("Level");
            }
        }

        private Class userClass;
        public Class UserClass
        {
            get { return userClass; }
            set
            {
                if (userClass == value) return;
                userClass = value;
                NotifyPropertyChanged("UserClass");
            }
        }

        private bool online;
        public bool Online
        {
            get
            {
                return online;
            }
            set
            {
                if (online == value) return;
                online = value;
                if (!online) { CurrentHP = 0; CurrentMP = 0; }
                NotifyPropertyChanged("Online");
            }
        }

        private uint serverId;
        public uint ServerId
        {
            get { return serverId; }
            set
            {
                if (serverId == value) return;
                serverId = value;
                NotifyPropertyChanged("ServerId");
            }
        }

        private uint playerId;
        public uint PlayerId
        {
            get { return playerId; }
            set
            {
                if (playerId == value) return;
                playerId = value;
                NotifyPropertyChanged("PlayerId");
            }
        }

        private int order;
        public int Order
        {
            get { return order; }
            set
            {
                if (order == value) return;
                order = value;
                NotifyPropertyChanged("Order");
            }
        }

        private bool canInvite;
        public bool CanInvite
        {
            get { return canInvite; }
            set
            {
                if (canInvite == value) return;
                canInvite = value;
                NotifyPropertyChanged("CanInvite");
            }
        }

        private Laurel laurel;
        public Laurel Laurel
        {
            get { return laurel; }
            set
            {
                if (laurel == value) return;
                laurel = value;
                NotifyPropertyChanged("Laurel");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private int currentHP;
        public int CurrentHP
        {
            get { return currentHP; }
            set
            {
                if (currentHP == value) return;
                currentHP = value;
                NotifyPropertyChanged("CurrentHP");
                NotifyPropertyChanged("HpFactor");
            }
        }

        private int maxHP;
        public int MaxHP
        {
            get { return maxHP; }
            set
            {
                if (maxHP == value) return;
                maxHP = value;
                NotifyPropertyChanged("MaxHP");
                NotifyPropertyChanged("HpFactor");
            }
        }

        private int currentMP;
        public int CurrentMP
        {
            get { return currentMP; }
            set
            {
                if (currentMP == value) return;
                currentMP = value;
                NotifyPropertyChanged("CurrentMP");
                NotifyPropertyChanged("MpFactor");
            }
        }

        private int maxMP;
        public int MaxMP
        {
            get { return maxMP; }
            set
            {
                if (maxMP == value) return;
                maxMP = value;
                NotifyPropertyChanged("MaxMP");
                NotifyPropertyChanged("MpFactor");
            }
        }

        private double hpFactor;
        public double HpFactor
        {
            get
            {
                if (maxHP > 0)
                {
                    hpFactor = (double)currentHP / (double)maxHP > 1 ? 1 : (double)currentHP / (double)maxHP;
                    return hpFactor;
                }
                else
                {
                    hpFactor = 1;
                    return hpFactor;
                }
            }
            set
            {
                if (hpFactor == value) return;
                hpFactor = value;
                NotifyPropertyChanged("HpFactor");
            }
        }

        private double mpFactor;
        public double MpFactor
        {
            get
            {
                if (maxMP > 0)
                {
                    mpFactor = (double)currentMP / (double)maxMP > 1 ? 1 : (double)currentMP / (double)maxMP;
                    return mpFactor;
                }
                else
                {
                    mpFactor = 1;
                    return mpFactor;
                }
            }
            set
            {
                if (mpFactor == value) return;
                mpFactor = value;
                NotifyPropertyChanged("MpFactor");
            }
        }

        private ReadyStatus ready = ReadyStatus.None;
        public ReadyStatus Ready
        {
            get { return ready; }
            set
            {
                if (ready == value) return;
                ready = value;
                NotifyPropertyChanged("Ready");
            }
        }

        private bool alive = true;
        public bool Alive
        {
            get { return alive; }
            set
            {
                if (alive == value) return;
                alive = value;
                NotifyPropertyChanged("Alive");
            }
        }

        private int rollResult = 0;
        public int RollResult
        {
            get { return rollResult; }
            set
            {
                if (rollResult == value) return;
                rollResult = value;
                if (rollResult == -1) IsRolling = false;
                NotifyPropertyChanged("RollResult");
            }
        }

        private bool isRolling = false;
        public bool IsRolling
        {
            get { return isRolling; }
            set
            {
                if (isRolling == value) return;
                isRolling = value;
                NotifyPropertyChanged("IsRolling");
            }
        }

        private bool isWinning = false;
        public bool IsWinning
        {
            get { return isWinning; }
            set
            {
                if (isWinning == value) return;
                isWinning = value;
                NotifyPropertyChanged("IsWinning");
            }
        }

        private bool isLeader = false;
        public bool IsLeader
        {
            get
            {
                return isLeader;
            }
            set
            {
                if (isLeader == value) return;
                isLeader = value;
                NotifyPropertyChanged("IsLeader");
            }
        }

        private List<uint> _debuffList = new List<uint>();
        public bool IsDebuffed
        {
            get => _debuffList.Count == 0 ? false : true;
        }
        public bool HasAggro
        {
            get => _hasAggro; set
            {
                if (_hasAggro == value) return;
                _hasAggro = value;
                NotifyPropertyChanged(nameof(HasAggro));
            }
        }
        private bool _hasAggro = false;


        private SynchronizedObservableCollection<AbnormalityDuration> _buffs;
        public SynchronizedObservableCollection<AbnormalityDuration> Buffs
        {
            get { return _buffs; }
            set
            {
                if (_buffs == value) return;
                _buffs = value;
            }
        }
        private SynchronizedObservableCollection<AbnormalityDuration> _debuffs;
        public SynchronizedObservableCollection<AbnormalityDuration> Debuffs
        {
            get { return _debuffs; }
            set
            {
                if (_debuffs == value) return;
                _debuffs = value;
            }
        }


        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks, double size, double margin)
        {
            if (SettingsManager.IgnoreAllBuffsInGroupWindow) return;
            if (SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow) return;
            if (SettingsManager.IgnoreAllBuffsInGroupWindow && ab.Type == AbnormalityType.Buff) return;


            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Priest:
                    if (FilterPriest(ab)) return;
                    break;
                case Class.Elementalist:
                    if (FilterMystic(ab)) return;
                    break;
                default:
                    return;
            }
            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {

                var newAb = new AbnormalityDuration(ab, duration, stacks, this.EntityId, _dispatcher, false, size * .9, size, new Thickness(margin, 1, 1, 1));
                Buffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        public void AddOrRefreshDebuff(Abnormality ab, uint duration, int stacks, double size, double margin)
        {
            if (!ab.IsBuff && !_debuffList.Contains(ab.Id))
            {
                _debuffList.Add(ab.Id);
                NotifyPropertyChanged("IsDebuffed");
            }

            if (SettingsManager.IgnoreRaidAbnormalitiesInGroupWindow) return;
            var existing = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, this.EntityId, _dispatcher, false, size * .9, size, new Thickness(margin, 1, 1, 1));

                Debuffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }

        private bool FilterPriest(Abnormality ab)
        {
            if (!Priest.CommonBuffs.Any(x => x == ab.Id)) return true;
            else return false;
        }
        private bool FilterMystic(Abnormality ab)
        {
            if (!Mystic.CommonBuffs.Any(x => x == ab.Id)) return true;
            else return false;
        }

        public void RemoveBuff(Abnormality ab)
        {
            var buff = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Buffs.Remove(buff);
            buff.Dispose();
        }
        public void RemoveDebuff(Abnormality ab)
        {
            if (!ab.IsBuff)
            {
                _debuffList.Remove(ab.Id);
                NotifyPropertyChanged("IsDebuffed");
            }
            var buff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Debuffs.Remove(buff);
            buff.Dispose();
        }
        public void ClearAbnormalities()
        {
            foreach (var item in _buffs)
            {
                item.Dispose();
            }
            foreach (var item in _debuffs)
            {
                item.Dispose();
            }
            _buffs.Clear();
            _debuffs.Clear();
            _debuffList.Clear();
        }

        public User(Dispatcher d)
        {
            _dispatcher = d;
            _debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(d);
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(d);
        }
    }
}