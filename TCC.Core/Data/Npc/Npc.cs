using Nostrum;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TCC.Data.Abnormalities;
using TCC.UI;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.Data.Npc;

public class Npc : ThreadSafeObservableObject, IDisposable
{
    public event Action<double>? HpFactorChanged;
    public event Action? DeleteEvent;

    string _name = "";
    bool _enraged;
    double _maxHP;
    double _currentHP;
    uint _maxShield;
    double _currentShield;
    bool _visible;
    ulong _target;
    bool _isSelected = true;
    int _remainingEnrageTime;
    bool _isBoss;
    double _hPFactor;
    double _currentPercentage;
    AggroCircle _currentAggroType = AggroCircle.None;

    public bool HasGage { get; set; }
    public ICommand Override { get; }
    public ICommand Blacklist { get; }
    public ulong EntityId { get; }
    public string Name
    {
        get => _name;
        set => RaiseAndSetIfChanged(value, ref _name);
    }
    public bool IsBoss
    {
        get => _isBoss;
        set => RaiseAndSetIfChanged(value, ref _isBoss);

    }
    public bool Enraged
    {
        get => _enraged;
        set => RaiseAndSetIfChanged(value, ref _enraged);

    }
    public double MaxHP
    {
        get => _maxHP;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxHP)) return;
            EnragePattern?.Update(value);
            HPFactor = MathUtils.FactorCalc(_currentHP, _maxHP);
        }
    }
    public double CurrentHP
    {
        get => _currentHP;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _currentHP)) return;
            HPFactor = MathUtils.FactorCalc(_currentHP, _maxHP);
        }
    }
    public uint MaxShield
    {
        get => _maxShield;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxShield)) return;
            InvokePropertyChanged(nameof(ShieldFactor));
        }
    }
    public double CurrentShield
    {
        get => _currentShield;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _currentShield)) return;
            InvokePropertyChanged(nameof(ShieldFactor));
        }
    }
    public double ShieldFactor => MaxShield > 0 ? CurrentShield / MaxShield : 0;
    public double HPFactor
    {
        get => _hPFactor;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hPFactor)) return;
            CurrentPercentage = HPFactor * 100;
            HpFactorChanged?.Invoke(_hPFactor);
        }
    }
    public double CurrentPercentage
    {
        get => _currentPercentage;
        private set => RaiseAndSetIfChanged(value, ref _currentPercentage);
    }
    public bool Visible
    {
        get => _visible;
        set => RaiseAndSetIfChanged(value, ref _visible);
    }
    public bool IsSelected
    {
        get => _isSelected;
        set => RaiseAndSetIfChanged(value, ref _isSelected);
    }
    public int RemainingEnrageTime
    {
        get => _remainingEnrageTime;
        set => RaiseAndSetIfChanged(value, ref _remainingEnrageTime);
    }
    public ulong Target
    {
        get => _target;
        set => RaiseAndSetIfChanged(value, ref _target);
    }
    public AggroCircle CurrentAggroType
    {
        get => _currentAggroType;
        set => RaiseAndSetIfChanged(value, ref _currentAggroType);
    }
    public uint ZoneId { get; }
    public uint TemplateId { get; }
    public Species Species { get; set; }
    public EnragePattern? EnragePattern { get; private set; }
    public TimerPattern? TimerPattern { get; private set; }
    public bool IsTower => TccUtils.IsGuildTower(ZoneId, TemplateId);
    public bool IsPhase1Dragon => TccUtils.IsPhase1Dragon(ZoneId, TemplateId);
    public uint GuildId //todo: inheritance when?
    {
        get
        {
            WindowManager.ViewModels.NpcVM.GuildIds.TryGetValue(EntityId, out var val); // todo: also no pls
            return val;
        }
    }
    public ThreadSafeObservableCollection<AbnormalityDuration> Buffs { get; }

    public Npc(ulong eId, uint zId, uint tId, bool boss, bool visible, EnragePattern? ep = null, TimerPattern? tp = null)
    {
        _dispatcher = WindowManager.ViewModels.NpcVM.Dispatcher;
        Buffs = new ThreadSafeObservableCollection<AbnormalityDuration>(_dispatcher);
        Game.DB!.MonsterDatabase.TryGetMonster(tId, zId, out var monster);
        Name = monster.Name;
        MaxHP = monster.MaxHP;
        Species = monster.Species;
        EntityId = eId;
        ZoneId = zId;
        TemplateId = tId;
        IsBoss = boss;
        Visible = visible;
        CurrentHP = MaxHP;
        EnragePattern = ep ?? new EnragePattern(10, 36);
        TimerPattern = tp;
        TimerPattern?.SetTarget(this);
        //if (IsPhase1Dragon)
        //    _shieldDuration = new Timer { Interval = NpcWindowViewModel.Ph1ShieldDuration * 1000 };
        //    _shieldDuration.Elapsed += ShieldFailed;
        //}
        Override = new RelayCommand(_ =>
        {
            Game.DB.MonsterDatabase.ToggleOverride(ZoneId, TemplateId, !IsBoss);
        }, _ => true);
        Blacklist = new RelayCommand(_ =>
        {
            Game.DB.MonsterDatabase.Blacklist(ZoneId, TemplateId, true);
        }, _ => true);
    }

    public void AddorRefreshAbnormality(Abnormality ab, uint duration, int stacks)
    {
        var existing = Buffs.ToSyncList().FirstOrDefault(x => x.Abnormality.Id == ab.Id);
        if (existing != null)
        {
            existing.Duration = duration;
            existing.DurationLeft = duration;
            existing.Stacks = stacks;
            existing.Refresh();
        }
        else
        {
            var newAb = new AbnormalityDuration(ab, duration, stacks, _target, _dispatcher, true);
            if (ab.Infinity) Buffs.Insert(0, newAb);
            else Buffs.Add(newAb);

            if (!ab.IsShield) return;

            MaxShield = ab.ShieldSize;
            CurrentShield = ab.ShieldSize;
        }
    }

    public void EndBuff(Abnormality ab)
    {
        try
        {
            var buff = Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id);

            if (buff == null) return;

            Buffs.Remove(buff);
            buff.Dispose();

            if (!ab.IsShield) return;

            CurrentShield = 0;
            MaxShield = 0;
        }
        catch
        {
            // ignored
        }
    }

    public override string ToString()
    {
        return $"{EntityId} - {Name}";
    }

    public void Dispose()
    {
        foreach (var buff in Buffs)
        {
            buff.Dispose();
        }

        TimerPattern?.Dispose();
        DeleteEvent?.Invoke();
    }

    public void Delete()
    {
        foreach (var buff in Buffs)
        {
            buff.Dispose();
        }

        DeleteEvent?.Invoke();
    }

    //////////////////SHIELD////////////////////////
    //TODO: make this a separate class
    //private readonly Timer _shieldDuration;

    ShieldStatus _shield = ShieldStatus.Off;

    public ShieldStatus Shield
    {
        get => _shield;
        set => RaiseAndSetIfChanged(value, ref _shield);
    }

    /*
            private void ShieldFailed(object sender, EventArgs e)
            {
                //_shieldDuration.Stop();
                Shield = ShieldStatus.Failed;
            }
    */

    public void BreakShield()
    {
        //_shieldDuration.Stop();
        Shield = ShieldStatus.Broken;
        Task.Delay(5000).ContinueWith(_ =>
        {
            Shield = ShieldStatus.Off;
        });
    }

    public void StartShield()
    {
        //_shieldDuration.Start();
        Shield = ShieldStatus.On;
    }

    public void SetTimerPattern() // todo: get these from somewhere instead of hardcoding
    {
        if (App.Settings.EthicalMode) return;

        // vergos ph4
        if (TemplateId == 4000 && ZoneId == 950) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        // nightmare kylos
        if (TemplateId == 3000 && ZoneId == 982) TimerPattern = new HpTriggeredTimerPattern(8 * 60, .9f);
        // nightmare antaroth
        if (TemplateId == 3000 && ZoneId == 920) TimerPattern = new HpTriggeredTimerPattern(5 * 60, .5f);
        // bahaar
        if (TemplateId == 2000 && ZoneId == 444) TimerPattern = new HpTriggeredTimerPattern(5 * 60, .3f);
        // dreadspire
        if (TemplateId == 1000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 2000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 3000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 4000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 5000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 6000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 7000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 8000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 9000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        if (TemplateId == 10000 && ZoneId == 434) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
        // nightmare gossamer regent
        if (TemplateId == 2000 && ZoneId == 3201) TimerPattern = new HpTriggeredTimerPattern(10 * 60, 0.75f);

        TimerPattern?.SetTarget(this);
    }

    public void SetEnragePattern()
    {
        if (App.Settings.EthicalMode)
        {
            EnragePattern = new EnragePattern(0, 0);
            return;
        }
        if (IsPhase1Dragon) EnragePattern = new EnragePattern(14, 50);
        if (ZoneId == 950 && !IsPhase1Dragon) EnragePattern = new EnragePattern(0, 0);
        if (ZoneId == 450 && TemplateId == 1003) EnragePattern = new EnragePattern((long)MaxHP, 600000000, 72);

        //ghilli
        if (TemplateId == 81301 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 65, int.MaxValue) { StaysEnraged = true };
        if (TemplateId == 81312 && ZoneId == 713) EnragePattern = new EnragePattern(0, 0);
        if (TemplateId == 81398 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 25, int.MaxValue) { StaysEnraged = true };
        if (TemplateId == 81399 && ZoneId == 713) EnragePattern = new EnragePattern(100 - 25, int.MaxValue) { StaysEnraged = true };

        if (ZoneId == 620 && TemplateId == 1000) EnragePattern = new EnragePattern((long)MaxHP, 420000000, 36);
        if (ZoneId == 622 && TemplateId == 1000) EnragePattern = new EnragePattern((long)MaxHP, 480000000, 36);
        if (ZoneId == 628)
        {
            if (TemplateId == 1000) EnragePattern = new EnragePattern(0, 0);
            if (TemplateId is 3000 or 3001) EnragePattern = new EnragePattern(10, 36);
        }

        if (TccUtils.IsFieldBoss(ZoneId, TemplateId)) EnragePattern = new EnragePattern(0, 0);
    }
}