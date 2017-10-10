using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace TCC.Data
{
    public class Player : TSPropertyChanged
    {
        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        ulong entityId;
        public ulong EntityId
        {
            get
            {
                return entityId;
            }
            set
            {
                if (entityId != value)
                {
                    entityId = value;
                    NotifyPropertyChanged("EntityId");
                }
            }
        }
        public uint PlayerId { get; internal set; }
        public uint ServerId { get; internal set; }

        Class playerclass;
        public Class Class
        {
            get
            {
                return playerclass;
            }
            set
            {
                if (playerclass != value)
                {
                    playerclass = value;
                    NotifyPropertyChanged("Class");
                }
            }
        }

        Laurel laurel;
        public Laurel Laurel
        {
            get
            {
                return laurel;
            }
            set
            {
                if (laurel != value)
                {
                    laurel = value;
                    NotifyPropertyChanged("Laurel");
                }
            }
        }

        int level;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (level != value)
                {
                    level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }

        private int itemLevel;
        public int ItemLevel
        {
            get => itemLevel;
            set
            {
                if (value != itemLevel)
                {
                    itemLevel = value;
                    NotifyPropertyChanged("ItemLevel");
                }
            }
        }

        private int maxHP;
        public int MaxHP
        {
            get { return maxHP; }
            set
            {
                if (maxHP != value)
                {
                    maxHP = value;
                    NotifyPropertyChanged("MaxHP");
                    NotifyPropertyChanged(nameof(HpFactor));

                }

            }
        }
        private int maxMP;
        public int MaxMP
        {
            get { return maxMP; }
            set
            {
                if (maxMP != value)
                {
                    maxMP = value;
                    NotifyPropertyChanged("MaxMP");
                    NotifyPropertyChanged(nameof(MpFactor));

                }

            }
        }
        private int maxST;
        public int MaxST
        {
            get { return maxST; }
            set
            {
                if (maxST != value)
                {
                    maxST = value;
                    NotifyPropertyChanged("MaxST");
                    NotifyPropertyChanged(nameof(StFactor));
                }
            }
        }

        private float currentHP;
        public float CurrentHP
        {
            get
            {
                return currentHP;
            }
            set
            {
                if (currentHP != value)
                {
                    currentHP = value;
                    NotifyPropertyChanged("CurrentHP");
                    NotifyPropertyChanged(nameof(HpFactor));
                }
            }
        }

        public double HpFactor => MaxHP > 0 ? CurrentHP / MaxHP : 1;
        public double MpFactor => MaxMP > 0 ? CurrentMP / MaxMP : 1;
        public double StFactor => MaxST > 0 ? CurrentST / MaxST : 1;

        private float currentMP;
        public float CurrentMP
        {
            get
            {
                return currentMP;
            }
            set
            {
                if (currentMP != value)
                {
                    currentMP = value;
                    NotifyPropertyChanged("CurrentMP");
                    NotifyPropertyChanged(nameof(MpFactor));

                }
            }
        }
        private float currentST;
        public float CurrentST
        {
            get
            {
                return currentST;
            }
            set
            {
                if (currentST != value)
                {
                    currentST = value;
                    NotifyPropertyChanged("CurrentST");
                    NotifyPropertyChanged(nameof(StFactor));

                }
            }
        }

        private float flightEnergy;
        public float FlightEnergy
        {
            get { return flightEnergy; }
            set
            {
                if (flightEnergy != value)
                {
                    flightEnergy = value;
                    NotifyPropertyChanged("FlightEnergy");
                }
            }
        }
        public float MaxFlightEnergy { get; } = 1000;

        private List<uint> _debuffList;
        internal void AddToDebuffList(Abnormality ab)
        {
            if (!ab.IsBuff && !_debuffList.Contains(ab.Id))
            {
                _debuffList.Add(ab.Id);
                NotifyPropertyChanged("IsDebuffed");
            }
        }
        internal void RemoveFromDebuffList(Abnormality ab)
        {
            if (ab.IsBuff == false)
            {
                _debuffList.Remove(ab.Id);
                NotifyPropertyChanged("IsDebuffed");
            }
        }
        public bool IsDebuffed
        {
            get => _debuffList.Count == 0 ? false : true;
        }

        bool isInCombat;
        public bool IsInCombat
        {
            get { return isInCombat; }
            set
            {
                if (value != isInCombat)
                {
                    isInCombat = value;
                    NotifyPropertyChanged("IsInCombat");
                }
            }
        }

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
        private SynchronizedObservableCollection<AbnormalityDuration> _infBuffs;
        public SynchronizedObservableCollection<AbnormalityDuration> InfBuffs
        {
            get { return _infBuffs; }
            set
            {
                if (_infBuffs == value) return;
                _infBuffs = value;
            }
        }


        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks, double size, double margin)
        {
            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

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

            var existing = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                Debuffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }
        public void AddOrRefreshInfBuff(Abnormality ab, uint duration, int stacks, double size, double margin)
        {
            var existing = InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                InfBuffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

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

            var buff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Debuffs.Remove(buff);
            buff.Dispose();

        }
        public void RemoveInfBuff(Abnormality ab)
        {
            var buff = InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            InfBuffs.Remove(buff);
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
            foreach (var item in _infBuffs)
            {
                item.Dispose();
            }
            _buffs.Clear();
            _debuffs.Clear();
            _infBuffs.Clear();
            _debuffList.Clear();
        }

        public Player()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _infBuffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _debuffList = new List<uint>();
        }
        public Player(ulong id, string name)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _buffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _infBuffs = new SynchronizedObservableCollection<AbnormalityDuration>(_dispatcher);
            _debuffList = new List<uint>();
            entityId = id;
            this.name = name;
        }
    }
}
