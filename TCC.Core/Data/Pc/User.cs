using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Abnormalities;
using TCC.Interop.Proxy;
using TCC.UI;
using TeraDataLite;

namespace TCC.Data.Pc;

//TODO: remove INPC from properties where it's not needed
public class User : ThreadSafeObservableObject
{
    ulong _entityId;
    uint _level;
    Class _userClass;
    Role _role;
    bool _online;
    uint _serverId;
    uint _playerId;
    int _order;
    bool _canInvite;
    Laurel _laurel;
    string _name = "";
    long _currentHp = 1;
    int _currentMp = 1;
    int _currentSt = 1;
    long _maxHp = 1;
    int _maxMp = 1;
    int _maxSt = 1;
    ReadyStatus _ready = ReadyStatus.None;
    bool _alive = true;
    int _rollResult;
    bool _isRolling;
    bool _isWinning;
    bool _isLeader;
    bool _hasAggro;
    string _location = "";
    readonly List<uint> _debuffList = [];
    GearItem? _weapon;
    GearItem? _armor;
    GearItem? _gloves;
    GearItem? _boots;
    bool _visible = true;
    bool _inRange;

    public ulong EntityId
    {
        get => _entityId;
        set => RaiseAndSetIfChanged(value, ref _entityId);
    }
    public uint Level
    {
        get => _level;
        set => RaiseAndSetIfChanged(value, ref _level);
    }
    public Class UserClass
    {
        get => _userClass;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _userClass)) return;

            Role = value switch
            {
                Class.Lancer => Role.Tank,
                Class.Priest => Role.Healer,
                Class.Mystic => Role.Healer,
                Class.Brawler => Role.Tank,
                _ => Role.Dps
            };
        }
    }
    public Role Role
    {
        get => _role;
        set => RaiseAndSetIfChanged(value, ref _role);
    }
    public bool Online
    {
        get => _online;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _online)) return;
            if (_online) return;

            CurrentHp = 0;
            CurrentMp = 0;
        }
    }
    public uint ServerId
    {
        get => _serverId;
        set => RaiseAndSetIfChanged(value, ref _serverId);
    }
    public uint PlayerId
    {
        get => _playerId;
        set => RaiseAndSetIfChanged(value, ref _playerId);
    }
    public int Order
    {
        get => _order;
        set => RaiseAndSetIfChanged(value, ref _order);
    }
    public bool CanInvite
    {
        get => _canInvite;
        set => RaiseAndSetIfChanged(value, ref _canInvite);
    }
    public Laurel Laurel
    {
        get => _laurel;
        set => RaiseAndSetIfChanged(value, ref _laurel);
    }
    public string Name
    {
        get => _name;
        set => RaiseAndSetIfChanged(value, ref _name);
    }
    public long CurrentHp
    {
        get => _currentHp;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _currentHp)) return;
            InvokePropertyChanged(nameof(HpFactor));
        }
    }
    public int CurrentMp
    {
        get => _currentMp;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _currentMp)) return;
            InvokePropertyChanged(nameof(MpFactor));
        }
    }
    public int CurrentSt
    {
        get => _currentSt;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _currentSt)) return;
            InvokePropertyChanged(nameof(StFactor));
        }
    }
    public long MaxHp
    {
        get => _maxHp;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxHp)) return;
            InvokePropertyChanged(nameof(HpFactor));
        }
    }
    public int MaxMp
    {
        get => _maxMp;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxMp)) return;
            InvokePropertyChanged(nameof(MpFactor));
        }
    }
    public int MaxSt
    {
        get => _maxSt;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxSt)) return;
            InvokePropertyChanged(nameof(StFactor));
        }
    }
    public double HpFactor => MathUtils.FactorCalc(CurrentHp, MaxHp);
    public double MpFactor => MathUtils.FactorCalc(CurrentMp, MaxMp);
    public double StFactor => MathUtils.FactorCalc(CurrentSt, MaxSt);
    public ReadyStatus Ready
    {
        get => _ready;
        set => RaiseAndSetIfChanged(value, ref _ready);
    }
    public bool Alive
    {
        get => _alive;
        set => RaiseAndSetIfChanged(value, ref _alive);
    }
    public int RollResult
    {
        get => _rollResult;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _rollResult)) return;
            if (_rollResult == -1) IsRolling = false;
        }
    }
    public bool IsRolling
    {
        get => _isRolling;
        set => RaiseAndSetIfChanged(value, ref _isRolling);
    }
    public bool IsWinning
    {
        get => _isWinning;
        set => RaiseAndSetIfChanged(value, ref _isWinning);
    }
    public bool IsLeader
    {
        get => _isLeader;
        set => RaiseAndSetIfChanged(value, ref _isLeader);

    }
    public bool IsPlayer => Name == Game.Me.Name;
    public bool IsDebuffed => _debuffList.Count != 0;
    public bool HasAggro
    {
        get => _hasAggro;
        set => RaiseAndSetIfChanged(value, ref _hasAggro);
    }
    public string Location
    {
        get => _location;
        set => RaiseAndSetIfChanged(value, ref _location);
    }
    public GearItem? Weapon
    {
        get => _weapon;
        set => RaiseAndSetIfChanged(value, ref _weapon);
    }
    public GearItem? Armor
    {
        get => _armor;
        set => RaiseAndSetIfChanged(value, ref _armor);
    }
    public GearItem? Gloves
    {
        get => _gloves;
        set => RaiseAndSetIfChanged(value, ref _gloves);
    }
    public GearItem? Boots
    {
        get => _boots;
        set => RaiseAndSetIfChanged(value, ref _boots);
    }
    public ThreadSafeObservableCollection<AbnormalityDuration> Buffs { get; }
    public ThreadSafeObservableCollection<AbnormalityDuration> Debuffs { get; }
    public bool Awakened { get; set; }
    public bool InCombat { get; set; } //make npc when needed
    public bool Visible
    {
        get => _visible;
        set => RaiseAndSetIfChanged(value, ref _visible);
    }
    public bool InRange
    {
        get => _inRange || IsPlayer;
        set => RaiseAndSetIfChanged(value, ref _inRange);
    }
    public ICommand RequestInteractiveCommand { get; }
    public ICommand AcceptApplyCommand { get; set; }
    public ICommand DeclineApplyCommand { get; set; }
    public ICommand InspectCommand { get; set; }

    public User(Dispatcher? d) : base(d)
    {
        Debuffs = new ThreadSafeObservableCollection<AbnormalityDuration>(_dispatcher);
        Buffs = new ThreadSafeObservableCollection<AbnormalityDuration>(_dispatcher);

        AcceptApplyCommand = new RelayCommand(_ =>
        {
            StubInterface.Instance.StubClient.GroupInviteUser(Name, Game.Group.IsRaid);
        });
        DeclineApplyCommand = new RelayCommand(_ =>
        {
            StubInterface.Instance.StubClient.DeclineUserGroupApply(PlayerId, ServerId);
            StubInterface.Instance.StubClient.RequestListingCandidates();
        });
        RequestInteractiveCommand = new RelayCommand(_ =>
        {
            WindowManager.ViewModels.PlayerMenuVM.Open(Name, ServerId, (int)Level);
        });
        InspectCommand = new RelayCommand(_ =>
        {
            StubInterface.Instance.StubClient.InspectUser(Name, ServerId);
        });

    }

    public User(GroupMemberData other, Dispatcher? d = null) : this(d)
    {
        PlayerId = other.PlayerId;
        UserClass = other.Class;
        Level = other.Level;
        Order = other.Order;
        Location = Game.DB!.GetSectionName(other.GuardId, other.SectionId);
        IsLeader = other.IsLeader;
        Online = other.Online;
        Name = other.Name;
        ServerId = other.ServerId == 0 ? Game.Me.ServerId : other.ServerId;
        EntityId = other.EntityId;
        CanInvite = other.CanInvite;
        Laurel = other.Laurel;
        Awakened = other.Awakened;
        Alive = other.Alive;
        CurrentHp = other.CurrentHP;
        CurrentMp = other.CurrentMP;
        MaxHp = other.MaxHP;
        MaxMp = other.MaxMP;
        CurrentSt = other.CurrentST;
        MaxSt = other.MaxST;
        InCombat = other.InCombat;

    }

    public void AddOrRefreshBuff(Abnormality ab, uint duration, int stacks)
    {
        if (!App.Settings.GroupWindowSettings.ShowAllAbnormalities
         && !App.Settings.GroupWindowSettings.GroupAbnormals[Class.Common].Contains(ab.Id)
         && !App.Settings.GroupWindowSettings.GroupAbnormals[Game.Me.Class].Contains(ab.Id))
        {
            return;
        }

        var buff = Buffs.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
        if (buff != null)
        {
            buff.Duration = duration;
            buff.DurationLeft = duration;
            buff.Stacks = stacks;
            buff.Refresh();
        }
        else
        {
            var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher,
                Game.Group.Size < App.Settings.GroupWindowSettings.DisableAbnormalitiesAnimationThreshold,
                App.Settings.GroupWindowSettings.Hidden.Contains(ab.Id));
            Buffs.Add(newAb);
        }
    }

    public void AddOrRefreshDebuff(Abnormality ab, uint duration, int stacks)
    {
        if (!ab.IsBuff && !_debuffList.Contains(ab.Id))
        {
            _debuffList.Add(ab.Id);
            InvokePropertyChanged(nameof(IsDebuffed));
        }

        var debuff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
        if (debuff == null)
        {
            var newAb = new AbnormalityDuration(ab, duration, stacks, EntityId, _dispatcher,
                Game.Group.Size < App.Settings.GroupWindowSettings.DisableAbnormalitiesAnimationThreshold,
                App.Settings.GroupWindowSettings.Hidden.Contains(ab.Id));

            Debuffs.Add(newAb);
        }
        else
        {
            debuff.Duration = duration;
            debuff.DurationLeft = duration;
            debuff.Stacks = stacks;
            debuff.Refresh();
        }
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
            InvokePropertyChanged(nameof(IsDebuffed));
        }
        var buff = Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);
        if (buff == null) return;
        
        Debuffs.Remove(buff);
        buff.Dispose();
    }

    public void ClearAbnormalities()
    {
        _dispatcher.Invoke(() =>
        {
            foreach (var item in Buffs)
            {
                item.Dispose();
            }
            foreach (var item in Debuffs)
            {
                item.Dispose();
            }
            Buffs.Clear();
            Debuffs.Clear();
            _debuffList.Clear();
        });
    }
}
