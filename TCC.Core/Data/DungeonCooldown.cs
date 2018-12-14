using System.Windows.Threading;

namespace TCC.Data
{
    public class DungeonCooldown : TSPropertyChanged
    {
        private int _entries;
        private int _total;

        public int Entries
        {
            get => _entries; set
            {
                if (_entries == value) return;
                _entries = value;
                N();
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

        public DungeonCooldown(Dungeon dung, Dispatcher d)
        {
            Dispatcher = d;
            Dungeon = dung;
            Entries = Runs;
        }

        public void Reset()
        {
            Entries = Runs;
        }

        public int Runs => Dungeon.ActualRuns;
    }
}