using System.Windows.Threading;
using TCC.Data.Pc;

namespace TCC.Data
{
    public class DungeonCooldown : TSPropertyChanged
    {
        private int _entries;
        private int _total;

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

        public int AvailableEntries
        {
            get
            {
                if (Dungeon.Cost == 0) return Entries;
                var res = (int)Owner.Coins / Dungeon.Cost;
                return res < Entries ? res : Entries;
            }
        }
        public int MaxAvailableEntries
        {
            get
            {
                if (Dungeon.Cost == 0) return Dungeon.ActualRuns;
                var res = (int)Owner.MaxCoins / Dungeon.Cost;
                if (Dungeon.ResetMode == ResetMode.Daily)
                {
                    return res > Dungeon.ActualRuns ? Dungeon.ActualRuns : res;
                }
                return res;
            }
        }

        public int Clears
        {
            get => _total;
            set
            {
                if (_total == value) return;
                _total = value;
                N();
            }
        }
        public Dungeon Dungeon { get; set; }

        //used only for filtering
        public ItemLevelTier RequiredIlvl => Dungeon.RequiredIlvl;
        public int Index => Dungeon.Index;

        public DungeonCooldown(Dungeon dung, Dispatcher d, Character owner)
        {
            Dispatcher = d;
            Dungeon = dung;
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

        public int Runs => Dungeon.ActualRuns;
        public Character Owner { get; set; }
    }
}