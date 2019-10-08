using System.Windows.Threading;
using FoglioUtils;
using Newtonsoft.Json;
using TCC.Data.Pc;

namespace TCC.Data
{
    public class DungeonCooldown : TSPropertyChanged
    {
        private int _entries;
        private int _clears;

        public uint Id { get; set; }
        public int Entries
        {
            get => _entries;
            set
            {
                if (_entries == value) return;
                _entries = value;
                N();
                N(nameof(AvailableEntries));
            }
        }
        public int Clears
        {
            get => _clears;
            set
            {
                if (_clears == value) return;
                _clears = value;
                N();
            }
        }

        [JsonIgnore]
        public int AvailableEntries
        {
            get
            {
                if (Dungeon.Cost == 0) return Entries;
                var res = (int)Owner.Coins / Dungeon.Cost;
                return res < Entries ? res : Entries;
            }
        }
        [JsonIgnore]
        public int MaxAvailableEntries
        {
            get
            {
                if (Dungeon.Cost == 0) return Dungeon.MaxEntries;
                var res = (int)Owner.MaxCoins / Dungeon.Cost;
                if (Dungeon.ResetMode == ResetMode.Daily)
                {
                    return res > Dungeon.MaxEntries ? Dungeon.MaxEntries : res;
                }
                return res;
            }
        }
        [JsonIgnore]
        public Dungeon Dungeon => Game.DB.DungeonDatabase.Dungeons.TryGetValue(Id, out var dg)
            ? dg
            : new Dungeon(0, "");
        [JsonIgnore]
        public int Runs => Dungeon.MaxEntries;
        [JsonIgnore]
        public Character Owner { get; set; }
        //used only for filtering
        //[JsonIgnore]
        //public ItemLevelTier RequiredIlvl => Dungeon.RequiredIlvl;
        [JsonIgnore]
        public int Index => Dungeon.Index;
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
            owner.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Character.Coins)) N(nameof(AvailableEntries));
                else if (args.PropertyName == nameof(Character.MaxCoins)) N(nameof(MaxAvailableEntries));
            };
        }

        public void Reset()
        {
            Entries = Runs;
        }

    }
}