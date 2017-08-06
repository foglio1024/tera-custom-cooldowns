using System;
using System.Windows.Threading;
using TCC.Data.Databases;
using TCC.ViewModels;

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
            if (Id == 9950) Entries = 0;
            else Entries = Convert.ToInt16(GetRuns());
        }

        internal int GetMaxBaseRuns()
        {
            return DungeonDatabase.Instance.DungeonDefinitions[Id].MaxBaseRuns;
        }
        public int GetRuns()
        {
            var bEntries = GetMaxBaseRuns();
            return SessionManager.IsElite ? bEntries * 2 : bEntries;
        }
    }
}
