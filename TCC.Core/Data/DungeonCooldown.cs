using System;
using System.Windows.Threading;
using TCC.Data.Databases;

namespace TCC.Data
{
    public class DungeonCooldown : TSPropertyChanged
    {
        short _entries;
        public uint Id { get; }
        public short Entries
        {
            get => _entries; set
            {
                if (_entries == value) return;
                _entries = value;
                NotifyPropertyChanged(nameof(Entries));
            }
        }
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
            return DungeonDatabase.Instance.DungeonDefinitions[Id].MaxBaseRuns;
        }
        public int GetRuns()
        {
            var bEntries = GetMaxBaseRuns();
            var eliteMultiplier = SessionManager.IsElite ? 2 : 1;

            return bEntries * eliteMultiplier;

        }
    }
}