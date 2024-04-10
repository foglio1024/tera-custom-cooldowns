using System;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using Nostrum;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Abnormalities;
using TCC.Data.Map;
using TeraDataLite;

namespace TCC.Data.Pc;

//TODO: remove INPC from properties where it's not needed
public class Character : ThreadSafeObservableObject, IComparable
{
    private string _name = "";
    private Class _class;
    private Laurel _laurel = Laurel.None;
    private bool _isLoggedIn;
    private bool _isSelected;
    private int _elleonMarks;
    private int _dragonwingScales;
    private int _piecesOfDragonScroll;
    private float _itemLevel;
    private int _level;
    private Location _lastLocation = new();
    private long _lastOnline;
    private string _serverName = "";
    private bool _hidden;
    private uint _coins;
    private uint _maxCoins;

    public uint Id { get; set; }
    public int Position { get; set; }
    public string GuildName { get; set; } = "";
    public string ServerName
    {
        get => _serverName;
        set => RaiseAndSetIfChanged(value, ref _serverName);
    }
    public string Name
    {
        get => _name; 
        set => RaiseAndSetIfChanged(value, ref _name);
    }
    public Class Class
    {
        get => _class; 
        set => RaiseAndSetIfChanged(value, ref _class);
    }
    public Laurel Laurel
    {
        get => _laurel; 
        set => RaiseAndSetIfChanged(value, ref _laurel);
    }
    public int Level
    {
        get => _level;
        set => RaiseAndSetIfChanged(value, ref _level);
    }
    public float ItemLevel
    {
        get => _itemLevel;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _itemLevel)) return;
            InvokePropertyChanged(nameof(ItemLevelTier));
        }
    }
    public uint Coins
    {
        get => _coins;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _coins)) return;
            DungeonInfo.UpdateAvailableEntries(_coins, _maxCoins);
        }
    }
    public uint MaxCoins
    {
        get => _maxCoins;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxCoins)) return;
            DungeonInfo.UpdateAvailableEntries(_coins, _maxCoins);
        }
    }
    public int ElleonMarks
    {
        get => _elleonMarks; set
        {
            if (!RaiseAndSetIfChanged(value, ref _elleonMarks)) return;
            InvokePropertyChanged(nameof(ElleonMarksFactor));
        }
    }
    public int DragonwingScales
    {
        get => _dragonwingScales;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _dragonwingScales)) return;
            InvokePropertyChanged(nameof(DragonwingScalesFactor));
        }
    }
    public int PiecesOfDragonScroll
    {
        get => _piecesOfDragonScroll;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _piecesOfDragonScroll)) return;
            InvokePropertyChanged(nameof(PiecesOfDragonScrollFactor));
        }
    }
    public long LastOnline
    {
        get => _lastOnline;
        set => RaiseAndSetIfChanged(value, ref _lastOnline);
    }
    public Location LastLocation
    {
        get => _lastLocation;
        set => RaiseAndSetIfChanged(value, ref _lastLocation);
    }
    public bool Hidden
    {
        get => _hidden; 
        set => RaiseAndSetIfChanged(value, ref _hidden);
    }
    public GuardianInfo GuardianInfo { get; }
    public VanguardInfo VanguardInfo { get; }
    public DungeonInfo DungeonInfo { get; }
    public ThreadSafeObservableCollection<AbnormalityData> Buffs { get; }
    public ThreadSafeObservableCollection<InventoryItem> Inventory { get; }
    [JsonIgnore]
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => RaiseAndSetIfChanged(value, ref _isLoggedIn);
    }
    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set => RaiseAndSetIfChanged(value, ref _isSelected);
    }
    [JsonIgnore]
    public ItemLevelTier ItemLevelTier
    {
        get
        {
            var tiers = Enum.GetValues(typeof(ItemLevelTier)).Cast<ItemLevelTier>().ToList();
            var ret = ItemLevelTier.Tier0;
            foreach (var t in tiers.Where(t => _itemLevel >= (int)t))
            {
                ret = t;
            }

            return ret;
        }
    }
    [JsonIgnore]
    public float ElleonMarksFactor => (float)MathUtils.FactorCalc(ElleonMarks, 1000);
    [JsonIgnore]
    public float DragonwingScalesFactor => (float)MathUtils.FactorCalc(DragonwingScales, 10);
    [JsonIgnore]
    public float PiecesOfDragonScrollFactor => (float)MathUtils.FactorCalc(PiecesOfDragonScroll, 40);
    [JsonIgnore]
    public ICommand UnhideCommand { get; }

    public Character()
    {
        Buffs = new ThreadSafeObservableCollection<AbnormalityData>(_dispatcher);
        Inventory = new ThreadSafeObservableCollection<InventoryItem>(_dispatcher);
        GuardianInfo = new GuardianInfo();
        VanguardInfo = new VanguardInfo();
        DungeonInfo = new DungeonInfo();
        UnhideCommand = new RelayCommand(_ => Hidden = false);
    }

    public Character(string name, Class c, uint id, int pos) : this()
    {
        Name = name;
        Class = c;
        Id = id;
        Position = pos;
    }

    public Character(CharacterData item) : this()
    {
        Id = item.Id;
        Class = item.CharClass;
        Level = item.Level;
        LastLocation = new Location(item.LastWorldId, item.LastGuardId, item.LastSectionId);
        LastOnline = item.LastOnline;
        Laurel = item.Laurel;
        Position = item.Position;
        Name = item.Name;
        GuildName = item.GuildName;
    }

    public void ResetDailyData()
    {
        VanguardInfo.DailiesDone = 0;
        GuardianInfo.Claimed = 0;
        DungeonInfo.ResetAll(ResetMode.Daily);
    }

    int IComparable.CompareTo(object? obj)
    {
        var ch = (Character?)obj;
        var pos = ch?.Position ?? 0;
        return Position.CompareTo(pos);
    }
}