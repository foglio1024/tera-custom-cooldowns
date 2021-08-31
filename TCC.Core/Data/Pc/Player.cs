using Nostrum;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using TCC.Data.Abnormalities;
using TCC.Utils;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC.Data.Pc
{
    //TODO: remove INPC from properties where it's not needed

    public class Player : ThreadSafePropertyChanged
    {
        public event Action? Death;
        public event Action? Ress;
        public event Action? CoinsUpdated;

        private string _name = "";
        private ulong _entityId;
        private Class _playerclass = Class.None;
        private Laurel _laurel = Laurel.None;
        private int _level;
        private float _itemLevel;
        private float _currentHP;
        private float _currentMP;
        private float _currentST;
        private long _maxHP;
        private int _maxMP;
        private int _maxST;
        private uint _maxShield;
        private float _currentShield;
        private float _flightEnergy;
        private bool _isInCombat;
        private float _critFactor;

        private bool _fire;
        private bool _ice;
        private bool _arcane;
        private bool _fireBoost;
        private bool _iceBoost;
        private bool _arcaneBoost;
        private bool _isAlive;
        private uint _coins;
        private uint _maxCoins;
        private readonly List<uint> _debuffList = new();
        private readonly Dictionary<uint, uint> _shields = new();

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                N();
            }
        }
        public ulong EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId == value) return;
                _entityId = value;
                N();
            }
        }
        public uint PlayerId { get; internal set; }
        public uint ServerId { get; internal set; }
        public Class Class
        {
            get => _playerclass;
            set
            {
                if (_playerclass == value) return;
                _playerclass = value;
                N();
            }
        }
        public Laurel Laurel
        {
            get => _laurel;
            set
            {
                if (_laurel == value) return;
                _laurel = value;
                N();
            }
        }
        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                N();
            }
        }
        public float ItemLevel
        {
            get => _itemLevel;
            set
            {
                if (value == _itemLevel) return;
                _itemLevel = value;
                N();
            }
        }
        public float CurrentHP
        {
            get => _currentHP;
            set
            {
                if (_currentHP == value) return;
                _currentHP = value;
                N(nameof(CurrentHP));
                N(nameof(TotalHP));
                N(nameof(HpFactor));
            }
        }
        public float CurrentMP
        {
            get => _currentMP;
            set
            {
                if (_currentMP == value) return;
                _currentMP = value;
                N();
                N(nameof(MpFactor));
            }
        }
        public float CurrentST
        {
            get => _currentST;
            set
            {
                if (_currentST == value) return;
                _currentST = value;
                N();
                N(nameof(StFactor));
            }
        }
        public long MaxHP
        {
            get => _maxHP;
            set
            {
                if (_maxHP == value) return;
                _maxHP = value;
                N();
                N(nameof(HpFactor));

            }
        }
        public int MaxMP
        {
            get => _maxMP;
            set
            {
                if (_maxMP == value) return;
                _maxMP = value;
                N();
                N(nameof(MpFactor));

            }
        }
        public int MaxST
        {
            get => _maxST;
            set
            {
                if (_maxST == value) return;
                _maxST = value;
                N();
                N(nameof(StFactor));
            }
        }
        public uint MaxShield
        {
            get => _maxShield;
            private set
            {
                if (_maxShield == value) return;
                _maxShield = value;
                N(nameof(MaxShield));
                N(nameof(ShieldFactor));
                N(nameof(HasShield));
            }
        }
        public double HpFactor => MaxHP > 0 ? CurrentHP / MaxHP : 1;
        public double MpFactor => MaxMP > 0 ? CurrentMP / MaxMP : 1;
        public double StFactor => MaxST > 0 ? CurrentST / MaxST : 1;
        public double ShieldFactor => MaxShield > 0 ? CurrentShield / MaxShield : 0;
        public bool HasShield => ShieldFactor > 0;
        public float TotalHP => CurrentHP + CurrentShield;
        public float CurrentShield
        {
            get => _currentShield;
            private set
            {
                if (_currentShield == value) return;
                if (value < 0) return;
                _currentShield = value;
                N(nameof(CurrentShield));
                N(nameof(TotalHP));
                N(nameof(ShieldFactor));
                N(nameof(HasShield));
            }
        }
        public float FlightEnergy
        {
            get => _flightEnergy;
            set
            {
                if (_flightEnergy == value) return;
                _flightEnergy = value;
                N();
            }
        }
        public int MagicalResistance { get; set; }

        public uint Coins
        {
            get => _coins;
            set
            {
                if (_coins == value) return;
                _coins = value;
                if (_coins == _maxCoins)
                {
                    Log.N("TCC", "Adventure coins maxed!", NotificationType.Success);
                    ChatManager.Instance.AddChatMessage(ChatManager.Instance.Factory.CreateMessage(ChatChannel.Notify, "System", "Adventure coins maxed!"));
                }

                N();
                N(nameof(CoinsFactor));
                CoinsUpdated?.Invoke();

            }
        }
        public uint MaxCoins
        {
            get => _maxCoins;
            set
            {
                if (_maxCoins == value) return;
                _maxCoins = value;
                N();
                N(nameof(CoinsFactor));
                CoinsUpdated?.Invoke();

            }
        }

        public double CoinsFactor => MathUtils.FactorCalc(_coins, _maxCoins);
        public bool IsDebuffed => _debuffList.Count != 0;
        public bool IsInCombat
        {
            get => _isInCombat;
            set
            {
                if (value == _isInCombat) return;
                _isInCombat = value;
                N();
            }
        }
        public bool IsAlive
        {
            get => _isAlive;
            internal set
            {
                if (_isAlive == value) return;
                _isAlive = value;
                if (value) Ress?.Invoke();
                else Death?.Invoke();
            }
        }
        public float CritFactor
        {
            get => _critFactor;
            set
            {
                if (_critFactor == value) return;
                _critFactor = value;
                N();
            }
        }
        public bool FireBoost
        {
            get => _fireBoost;
            set
            {
                if (_fireBoost == value) return;
                _fireBoost = value;
                N();
            }
        }
        public bool IceBoost
        {
            get => _iceBoost;
            set
            {
                if (_iceBoost == value) return;
                _iceBoost = value;
                N();
            }
        }
        public bool ArcaneBoost
        {
            get => _arcaneBoost;
            set
            {
                if (_arcaneBoost == value) return;
                _arcaneBoost = value;
                N();
            }
        }
        public bool Fire
        {
            get => _fire;
            set
            {
                if (_fire == value) return;
                _fire = value;
                N();
            }
        }
        public bool Ice
        {
            get => _ice;
            set
            {
                if (_ice == value) return;
                _ice = value;
                N();
            }
        }
        public bool Arcane
        {
            get => _arcane;
            set
            {
                if (_arcane == value) return;
                _arcane = value;
                N();
            }
        }

        public Counter StacksCounter { get; set; }

        // tracking only warrior stance for now
        public StanceTracker<WarriorStance> WarriorStance { get; set; }

        public ThreadSafeObservableCollection<AbnormalityDuration>? Buffs { get; private set; }
        public ThreadSafeObservableCollection<AbnormalityDuration>? Debuffs { get; private set; }
        public ThreadSafeObservableCollection<AbnormalityDuration>? InfBuffs { get; private set; }

        public Player()
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
            StacksCounter = new Counter(10, true);
            WarriorStance = new StanceTracker<WarriorStance>();
        }


        #region Shield
        public void DamageShield(uint damage)
        {
            _dispatcher?.Invoke(() =>
            {
                if (_shields.Count == 0) return;
                var firstShield = _shields.First();
                if (firstShield.Value >= damage)
                {
                    _shields[firstShield.Key] -= damage;
                }
                else
                {
                    _shields[firstShield.Key] = 0;
                }
                RefreshShieldAmount();
            });
        }
        private void AddShield(Abnormality ab)
        {
            _dispatcher?.Invoke(() =>
            {
                _shields[ab.Id] = GetShieldSize(ab);
                RefreshMaxShieldAmount();
                RefreshShieldAmount();
            });

            uint GetShieldSize(Abnormality a)
            {
                return Class switch
                {
                    Class.Sorcerer => Convert.ToUInt32(EpDataProvider.ManaBarrierMult * a.ShieldSize),
                    Class.Mystic when a.Id == 702001 => (a.ShieldSize + ((uint)MagicalResistance * 50 / 100)),
                    Class.Priest when a.Id == 800304 => (a.ShieldSize + ((uint)MagicalResistance * 65 / 100)),
                    _ when a.Id == 702001 || a.Id == 800304 => a.ShieldSize + 25000,
                    _ => a.ShieldSize
                };
            }
        }
        private void EndShield(Abnormality ab)
        {
            _dispatcher?.Invoke(() =>
            {
                _shields.Remove(ab.Id);
                RefreshShieldAmount();
            });
        }
        private void RefreshShieldAmount()
        {
            if (_shields.Count == 0)
            {
                CurrentShield = 0;
                MaxShield = 0;
                return;
            }
            _currentShield = 0;
            var total = 0U;
            foreach (var amount in _shields.Values)
            {
                total += amount;
            }
            CurrentShield = total;
        }
        private void RefreshMaxShieldAmount()
        {
            foreach (var amount in _shields.Values)
            {
                MaxShield += amount;
            }
        }
        #endregion

        #region Abnormalities
        public void InitAbnormalityCollections(Dispatcher disp)
        {
            Buffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
            Debuffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
            InfBuffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
            //_shields = new Dictionary<uint, uint>();
            //_debuffList = new List<uint>();

        }

        public void UpdateAbnormality(Abnormality ab, uint pDuration, int pStacks)
        {
            if (!App.Settings.BuffWindowSettings.Pass(ab)) return; // by HQ 
            FindAndUpdate(ab, pDuration, pStacks);
        }
        public void EndAbnormality(Abnormality ab)
        {
            if (!App.Settings.BuffWindowSettings.Pass(ab)) return; // by HQ 
            FindAndRemove(ab);
        }
        public void EndAbnormality(uint id)
        {
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(id, out var ab) || !ab.CanShow) return;
            if (!App.Settings.BuffWindowSettings.Pass(ab)) return; // by HQ 
            FindAndRemove(ab);
        }
        private void FindAndUpdate(Abnormality ab, uint duration, int stacks)
        {
            _dispatcher?.Invoke(() =>
            {
                var list = GetList(ab);
                var existing = list.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
                if (existing == null)
                {
                    var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, true);
                    list.Add(newAb);
                    if (ab.IsShield) AddShield(ab);
                    if (ab.IsDebuff) AddToDebuffList(ab);
                    return;
                }

                existing.Duration = duration;
                existing.DurationLeft = duration;
                existing.Stacks = stacks;
                existing.Refresh();
            });
        }
        private void FindAndRemove(Abnormality ab)
        {
            var list = GetList(ab);
            var target = list.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (target == null) return;
            list.Remove(target);
            target.Dispose();

            if (ab.IsShield) EndShield(ab);
            if (ab.IsDebuff) RemoveFromDebuffList(ab);
        }

        internal void AddToDebuffList(Abnormality ab)
        {
            if (ab.IsBuff || _debuffList.Contains(ab.Id)) return;
            _debuffList.Add(ab.Id);
            N(nameof(IsDebuffed));
        }

        internal void RemoveFromDebuffList(Abnormality ab)
        {
            if (ab.IsBuff) return;
            _debuffList.Remove(ab.Id);
            N(nameof(IsDebuffed));
        }

        public void ClearAbnormalities()
        {
            Buffs?.ToSyncList().ForEach(item => item.Dispose());
            Debuffs?.ToSyncList().ForEach(item => item.Dispose());
            InfBuffs?.ToSyncList().ForEach(item => item.Dispose());

            Buffs?.Clear();
            Debuffs?.Clear();
            InfBuffs?.Clear();

            _debuffList.Clear();

            CurrentShield = 0;
            N(nameof(IsDebuffed));
        }

        // utils
        private ThreadSafeObservableCollection<AbnormalityDuration> GetList(Abnormality ab)
        {
            var list = ab.Type switch
            {
                AbnormalityType.Debuff => Debuffs,
                AbnormalityType.DOT => Debuffs,
                AbnormalityType.Stun => Debuffs,
                AbnormalityType.Buff => (ab.Infinity ? InfBuffs : Buffs),
                AbnormalityType.Special => (ab.Infinity ? InfBuffs : Buffs),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (list == null)
                throw new InvalidOperationException("Invalid list type requested");

            return list;
        }
        #endregion

        #region Deprecated
        [Obsolete]
        public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks)
        {
            var existing = Buffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher!, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                Buffs?.Add(newAb);
                if (ab.IsShield)
                {
                    AddShield(ab);
                }

                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        [Obsolete]
        public void AddOrRefreshDebuff(Abnormality ab, uint duration, int stacks)
        {
            var existing = Debuffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher!, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                Debuffs?.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }
        [Obsolete]
        public void AddOrRefreshInfBuff(Abnormality ab, uint duration, int stacks)
        {
            var existing = InfBuffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (existing == null)
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher!, true/*, size * .9, size, new System.Windows.Thickness(margin)*/);

                InfBuffs?.Add(newAb);
                return;
            }
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();

        }
        [Obsolete]
        public void RemoveBuff(Abnormality ab)
        {
            var buff = Buffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;

            Buffs?.Remove(buff);
            buff.Dispose();

            if (ab.IsShield)
            {
                EndShield(ab);
            }

        }
        [Obsolete]
        public void RemoveDebuff(Abnormality ab)
        {

            var buff = Debuffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            Debuffs?.Remove(buff);
            buff.Dispose();
        }
        [Obsolete]
        public void RemoveInfBuff(Abnormality ab)
        {
            var buff = InfBuffs?.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (buff == null) return;
            InfBuffs?.Remove(buff);
            buff.Dispose();
        }

        #endregion

    }
}
