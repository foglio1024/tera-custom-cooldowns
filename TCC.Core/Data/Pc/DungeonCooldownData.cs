using Newtonsoft.Json;

namespace TCC.Data.Pc;

public class DungeonCooldownData
{
    uint _coins;
    uint _maxCoins;

    public uint Id { get; set; }
    public int Entries { get; set; }
    public int Clears { get; set; }
    [JsonIgnore]
    public Dungeon Dungeon => Game.DB!.DungeonDatabase.Dungeons.TryGetValue(Id, out var dg)
        ? dg
        : new Dungeon(Id, "");
    [JsonIgnore]
    public int AvailableEntries
    {
        get
        {
            if (Dungeon.Cost == 0) return Entries;
            var res = (int)_coins / Dungeon.Cost;
            return res < Entries ? res : Entries;
        }
    }
    [JsonIgnore]
    public int MaxAvailableEntries
    {
        get
        {
            if (Dungeon.Cost == 0) return Dungeon.MaxEntries;
            var res = (int)_maxCoins / Dungeon.Cost;
            if (Dungeon.ResetMode == ResetMode.Daily)
            {
                return res > Dungeon.MaxEntries ? Dungeon.MaxEntries : res;
            }
            return res;
        }
    }

    public DungeonCooldownData(uint id)
    {
        Id = id;
        Reset();
    }

    public void Reset()
    {
        Entries = Dungeon.MaxEntries;
    }

    public void UpdateAvailableEntries(uint coins, uint maxCoins)
    {
        _coins = coins;
        _maxCoins = maxCoins;
    }
}