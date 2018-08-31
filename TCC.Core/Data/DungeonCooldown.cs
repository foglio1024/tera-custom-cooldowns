using System;
using System.Windows.Threading;

namespace TCC.Data
{
    public class DungeonCooldown : TSPropertyChanged
    {
        private short _entries;
        private int _total;
        public uint Id { get; }
        public short Entries
        {
            get => _entries; set
            {
                if (_entries == value) return;
                _entries = value;
                NPC(nameof(Entries));
            }
        }
        public int Clears
        {
            get => _total;
            set
            {
                if (_total == value) return;
                _total = value;
                NPC(nameof(Clears));
            }
        }

        public DungeonTier Tier => SessionManager.DungeonDatabase.DungeonDefs[Id].Tier;


        public DungeonCooldown(uint id, Dispatcher d)
        {
            _dispatcher = d;
            Id = id;
            Entries = (short)GetRuns();
        }

        public void Reset()
        {
            Entries = Convert.ToInt16(GetRuns());
        }

        internal int GetMaxBaseRuns()
        {
            return SessionManager.DungeonDatabase.DungeonDefs[Id].MaxBaseRuns;
        }
        public int GetRuns()
        {
            var bEntries = GetMaxBaseRuns();
            var eliteMultiplier = SessionManager.IsElite ? 2 : 1;

            return bEntries * eliteMultiplier;

        }
    }
}