using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;
using System.ComponentModel;
using System.Windows.Threading;
using TCC.Data.Pc;

namespace TCC.Data;

public class DungeonCooldown : ThreadSafeObservableObject
{
    int _entries;

    uint Id { get; }
    public int Clears { get; }
    public int Entries
    {
        get => _entries;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _entries)) return;
            InvokePropertyChanged(nameof(AvailableEntries));
        }
    }
    [JsonIgnore]
    public int AvailableEntries
    {
        get
        {
            if (Dungeon.Cost == 0 || Owner == null) return Entries;
            var res = (int)Owner.Coins / Dungeon.Cost;
            return res < Entries ? res : Entries;
        }
    }
    [JsonIgnore]
    public int MaxAvailableEntries
    {
        get
        {
            if (Dungeon.Cost == 0 || Owner == null) return Dungeon.MaxEntries;

            var res = (int)Owner.MaxCoins / Dungeon.Cost;
            if (Dungeon.ResetMode == ResetMode.Daily)
            {
                return res > Dungeon.MaxEntries ? Dungeon.MaxEntries : res;
            }
            return res;
        }
    }
    [JsonIgnore]
    public Dungeon Dungeon => Game.DB!.DungeonDatabase.Dungeons.TryGetValue(Id, out var dg)
        ? dg
        : new Dungeon(0, "");
    [JsonIgnore]
    public int Runs => Dungeon.MaxEntries;
    [JsonIgnore]
    public Character? Owner { get; }
    [JsonIgnore]
    public int Index => Dungeon.Index;
    //used only for filtering
    //[JsonIgnore]
    //public ItemLevelTier RequiredIlvl => Dungeon.RequiredIlvl;

    [JsonConstructor]
    public DungeonCooldown(uint id, int entries, int clears)
    {
        Id = id;
        Entries = entries;
        Clears = clears;
    }

    public DungeonCooldown(uint id, Dispatcher d, Character owner)
    {
        Dispatcher = d;
        Id = id;
        Entries = Runs;
        Owner = owner;
        owner.PropertyChanged += OnOwnerPropertyChanged;
    }

    void OnOwnerPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(Character.Coins):
                InvokePropertyChanged(nameof(AvailableEntries));
                break;

            case nameof(Character.MaxCoins):
                InvokePropertyChanged(nameof(MaxAvailableEntries));
                break;
        }
    }

    public void Reset()
    {
        Entries = Runs;
    }
}