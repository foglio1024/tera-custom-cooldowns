using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using TCC.Data.Abnormalities;

namespace TCC.Data.Pc
{
    //TODO: remove INPC from properties where it's not needed

    public class Player : TSPropertyChanged
    {
        private float _critFactor;
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                NPC();
            }
        }

        private ulong _entityId;
        public ulong EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    NPC();
                }
            }
        }
        public uint PlayerId { get; internal set; }
        public uint ServerId { get; internal set; }

        private Class _playerclass = Class.None;
        public Class Class
        {
            get => _playerclass;
            set
            {
                if (_playerclass != value)
                {
                    _playerclass = value;
                    NPC();
                }
            }
        }

        private Laurel _laurel;
        public Laurel Laurel
        {
            get => _laurel;
            set
            {
                if (_laurel != value)
                {
                    _laurel = value;
                    NPC();
                }
            }
        }

        private int _level;
        public int Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    NPC();
                }
            }
        }

        private int _itemLevel;
        public int ItemLevel
        {
            get => _itemLevel;
            set
            {
                if (value != _itemLevel)
                {
                    _itemLevel = value;
                    NPC();
                }
            }
        }

        private long _maxHP;
        public long MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP != value)
                {
                    _maxHP = value;
                    NPC();
                    NPC(nameof(HpFactor));

                }

            }
        }
        private int _maxMP;
        public int MaxMP
        {
            get => _maxMP;
            set
            {
                if (_maxMP != value)
                {
                    _maxMP = value;
                    NPC();
                    NPC(nameof(MpFactor));

                }

            }
        }
        private int _maxST;
        public int MaxST
        {
            get => _maxST;
            set
            {
                if (_maxST == value) return;
                _maxST = value;
                NPC();
                NPC(nameof(StFactor));
            }
        }

        private uint _maxShield;
        public uint MaxShield
        {
            get => _maxShield;
            set
            {
                if (_maxShield != value)
                {
                    _maxShield = value;
                    NPC(nameof(MaxShield));
                    NPC(nameof(ShieldFactor));
                    NPC(nameof(HasShield));
                }
            }
        }

        private float _currentHP;
        public float CurrentHP
        {
            get => _currentHP;
            set
            {
                if (_currentHP == value) return;
                _currentHP = value;
                NPC(nameof(CurrentHP));
                NPC(nameof(TotalHP));
                NPC(nameof(HpFactor));
            }
        }

        public double HpFactor => MaxHP > 0 ? CurrentHP / MaxHP : 1;
        public double MpFactor => MaxMP > 0 ? CurrentMP / MaxMP : 1;
        public double StFactor => MaxST > 0 ? CurrentST / MaxST : 1;
        public double ShieldFactor => MaxShield > 0 ? CurrentShield / MaxShield : 0;

        public bool HasShield => ShieldFactor > 0;

        public float TotalHP => CurrentHP + CurrentShield;

        private float _currentMP;
        public float CurrentMP
        {
            get => _currentMP;
            set
            {
                if (_currentMP != value)
                {
                    _currentMP = value;
                    NPC();
                    NPC(nameof(MpFactor));

                }
            }
        }
        private float _currentST;
        public float CurrentST
        {
            get => _currentST;
            set
            {
                if (_currentST != value)
                {
                    _currentST = value;
                    NPC();
                    NPC(nameof(StFactor));

                }
            }
        }

        private float _currentShield;
        public float CurrentShield
        {
            get => _currentShield;
            set
            {
                if(_currentShield == value) return;
                if(value < 0) return;
                _currentShield = value;
                NPC(nameof(CurrentShield));
                NPC(nameof(TotalHP));
                NPC(nameof(ShieldFactor));
                NPC(nameof(HasShield));
            }
        }

        private float _flightEnergy;
        public float FlightEnergy
        {
            get => _flightEnergy;
            set
            {
                if (_flightEnergy == value) return;
                _flightEnergy = value;
                NPC();
            }
        }

        private readonly List<uint> _debuffList;
        internal void AddToDebuffList(Abnormality ab)
        {
            if (!ab.IsBuff && !_debuffList.Contains(ab.Id))
            {
                _debuffList.Add(ab.Id);
                NPC(nameof(IsDebuffed));
            }
        }
        internal void RemoveFromDebuffList(Abnormality ab)
        {
            if (ab.IsBuff == false)
            {
                _debuffList.Remove(ab.Id);
                NPC(nameof(IsDebuffed));
            }
        }
        public bool IsDebuffed => _debuffList.Count != 0;

        private bool _isInCombat;
        public bool IsInCombat
        {
            get => _isInCombat;
            set
            {
                if (value != _isInCombat)
                {
                    _isInCombat = value;
                    NPC();
                }
            }
        }

        public SynchronizedObservableCollection<AbnormalityDuration> Buffs { get; set; }

        public SynchronizedObservableCollection<AbnormalityDuration> Debuffs { get; set; }

        public SynchronizedObservableCollection<AbnormalityDuration> InfBuffs { get; set; }

        public float CritFactor
        {
            get => _critFactor;
            set
            {
                if(_critFactor == value) return;
                _critFactor = value;
                NPC();
            }
        }

        private bool _fireBoost;
        private bool _iceBoost;
        private bool _arcaneBoost;

        public bool FireBoost
        {
            get => _fireBoost;
            set
            {
                if (_fireBoost == value) return;
                _fireBoost = value;
                NPC();
            }
        }
        public bool IceBoost
        {
            get => _iceBoost;
            set
            {
                if (_iceBoost == value) return;
                _iceBoost = value;
                NPC();
            }
        }
        public bool ArcaneBoost
        {
            get => _arcaneBoost;
            set
            {
                if (_arcaneBoost == value) return;
                _arcaneBoost = value;
                NPC();
            }
        }


        private bool _fire;
        private bool _ice;
        private bool _arcane;

        public bool Fire
        {
            get => _fire;
            set
            {
                if (_fire == value) return;
                _fire = value;
                NPC();
            }
        }
        public bool Ice
        {
            get => _ice;
            set
            {
                if (_ice == value) return;
                _ice = value;
                NPC();
            }
        }
        public bool Arcane
        {
            get => _arcane;
            set
            {
                if (_arcane == value) return;
                _arcane = value;
                NPC();
            }
        }



        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks)
        {
            var existing = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, Dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                Buffs.Add(newAb);
                if (ab.IsShield)
                {
                    MaxShield = ab.ShieldSize;
                    CurrentShield = ab.ShieldSize;
                }

                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        public void AddOrRefreshDebuff(Abnormality ab, uint duration, int stacks)
        {

            var existing = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, Dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                Debuffs.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }
        public void AddOrRefreshInfBuff(Abnormality ab, uint duration, int stacks)
        {
            var existing = InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, Dispatcher, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

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

            if (ab.IsShield)
            {
                MaxShield = 0;
                CurrentShield = 0;
            }

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
            foreach (var item in Buffs)
            {
                item.Dispose();
            }
            foreach (var item in Debuffs)
            {
                item.Dispose();
            }
            foreach (var item in InfBuffs)
            {
                item.Dispose();
            }
            Buffs.Clear();
            Debuffs.Clear();
            InfBuffs.Clear();
            _debuffList.Clear();
            NPC(nameof(IsDebuffed));
        }

        public Player()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Buffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            Debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            InfBuffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            _debuffList = new List<uint>();
        }
        public Player(ulong id, string name)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Buffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            Debuffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            InfBuffs = new SynchronizedObservableCollection<AbnormalityDuration>(Dispatcher);
            _debuffList = new List<uint>();
            _entityId = id;
            _name = name;
        }
    }
}
