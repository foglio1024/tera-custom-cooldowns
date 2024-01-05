using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Abnormalities;
using TCC.Utils;
using TCC.ViewModels;
using TeraDataLite;

namespace TCC.Data.Pc;
//TODO: remove INPC from properties where it's not needed

public class Player : ThreadSafeObservableObject
{
    public event Action? Death;
    public event Action? Ress;
    public event Action? CoinsUpdated;

    string _name = "";
    ulong _entityId;
    Class _playerclass = Class.None;
    Laurel _laurel = Laurel.None;
    int _level;
    float _itemLevel;
    float _currentHP;
    float _currentMP;
    float _currentST;
    long _maxHP;
    int _maxMP;
    int _maxST;
    uint _maxShield;
    float _currentShield;
    float _flightEnergy;
    bool _isInCombat;
    float _critFactor;
    bool _fire; // todo: move class-specific stuff away from here
    bool _ice;
    bool _arcane;
    bool _fireBoost;
    bool _iceBoost;
    bool _arcaneBoost;
    bool _isAlive;
    uint _coins;
    uint _maxCoins;
    readonly List<uint> _debuffList = [];
    readonly Dictionary<uint, uint> _shields = [];
    readonly object _listLock = new();
    ThreadSafeObservableCollection<AbnormalityDuration> _buffs = [];
    ThreadSafeObservableCollection<AbnormalityDuration> _debuffs = [];
    ThreadSafeObservableCollection<AbnormalityDuration> _infBuffs = [];

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
            N();
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
            N();
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
            N();
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
                Log.N("TCC", "Adventure coins maxed!", NotificationType.Info);
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
    public Counter StacksCounter { get; set; } = new(10, true); // todo: move class-specific stuff away from here
    public StanceTracker<WarriorStance> WarriorStance { get; set; } = new(); // todo: move class-specific stuff away from here
    public ThreadSafeObservableCollection<AbnormalityDuration> Buffs
    {
        get
        {
            lock (_listLock)
            {
                return _buffs;
            }
        }
    }
    public ThreadSafeObservableCollection<AbnormalityDuration> Debuffs
    {
        get
        {
            lock (_listLock)
            {
                return _debuffs;
            }
        }
    }
    public ThreadSafeObservableCollection<AbnormalityDuration> InfBuffs
    {
        get
        {
            lock (_listLock)
            {
                return _infBuffs;
            }
        }
    }

    // todo: maybe all this logic should be in its own class too
    #region Shield 
    public void DamageShield(uint damage)
    {
        _dispatcher.Invoke(() =>
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

    void AddShield(Abnormality ab)
    {
        _dispatcher.Invoke(() =>
        {
            _shields[ab.Id] = GetShieldSize(ab);
            RefreshMaxShieldAmount();
            RefreshShieldAmount();
        });

        return;

        uint GetShieldSize(Abnormality a)
        {
            return Class switch
            {
                Class.Sorcerer => Convert.ToUInt32(EpDataProvider.ManaBarrierMult * a.ShieldSize),
                Class.Mystic when a.Id == 702001 => a.ShieldSize + (uint)MagicalResistance * 50 / 100,
                Class.Priest when a.Id == 800304 => a.ShieldSize + (uint)MagicalResistance * 65 / 100,
                _ when a.Id is 702001 or 800304 => a.ShieldSize + 25000,
                _ => a.ShieldSize
            };
        }
    }

    void EndShield(Abnormality ab)
    {
        _dispatcher.Invoke(() =>
        {
            _shields.Remove(ab.Id);
            RefreshShieldAmount();
        });
    }

    void RefreshShieldAmount()
    {
        if (_shields.Count == 0)
        {
            CurrentShield = 0;
            MaxShield = 0;
        }
        else
        {
            _currentShield = 0;
            var total = _shields.Values.Aggregate(0U, (current, amount) => current + amount);
            CurrentShield = total;
        }
    }

    void RefreshMaxShieldAmount()
    {
        foreach (var amount in _shields.Values)
        {
            MaxShield += amount;
        }
    }
    #endregion

    // todo: maybe all this logic should be in its own class too
    #region Abnormalities
    public void InitAbnormalityCollections(Dispatcher disp)
    {
        _buffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
        _debuffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
        _infBuffs = new ThreadSafeObservableCollection<AbnormalityDuration>(disp);
    }

    public void UpdateAbnormality(Abnormality ab, uint pDuration, int pStacks)
    {
        if (!App.Settings.BuffWindowSettings.Pass(ab)) return; // by HQ 

        lock (_listLock)
        {
            FindAndUpdateAbnormality(ab, pDuration, pStacks);
        }
    }

    public void EndAbnormality(Abnormality ab)
    {
        if (!App.Settings.BuffWindowSettings.Pass(ab)) return; // by HQ 

        lock (_listLock)
        {
            FindAndRemove(ab);
        }
    }

    void FindAndUpdateAbnormality(Abnormality ab, uint duration, int stacks)
    {
        _dispatcher.Invoke(() =>
        {
            var list = GetList(ab);
            var abnormality = list.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
            if (abnormality != null)
            {
                abnormality.Duration = duration;
                abnormality.DurationLeft = duration;
                abnormality.Stacks = stacks;
                abnormality.Refresh();
            }
            else
            {
                var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher, true, App.Settings.BuffWindowSettings.Hidden.Contains(ab.Id));
                list.Add(newAb);
                if (ab.IsShield) AddShield(ab);
                if (ab.IsDebuff) AddToDebuffList(ab);
            }
        });
    }

    void FindAndRemove(Abnormality ab)
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
        lock (_listLock)
        {
            _buffs.ToSyncList().ForEach(item => item.Dispose());
            _debuffs.ToSyncList().ForEach(item => item.Dispose());
            _infBuffs.ToSyncList().ForEach(item => item.Dispose());

            _buffs.Clear();
            _debuffs.Clear();
            _infBuffs.Clear();
        }

        _debuffList.Clear();
        N(nameof(IsDebuffed));

        CurrentShield = 0;
    }

    // utils
    ThreadSafeObservableCollection<AbnormalityDuration> GetList(Abnormality abnormality)
    {
        var list = abnormality.Type switch
        {
            AbnormalityType.Debuff => _debuffs,
            AbnormalityType.DOT => _debuffs,
            AbnormalityType.Stun => _debuffs,
            AbnormalityType.Buff => abnormality.Infinity ? _infBuffs : _buffs,
            AbnormalityType.Special => abnormality.Infinity ? _infBuffs : _buffs,
            _ => throw new ArgumentOutOfRangeException(nameof(abnormality))
        };

        return list ?? throw new InvalidOperationException("Invalid list type requested");
    }
    #endregion
}