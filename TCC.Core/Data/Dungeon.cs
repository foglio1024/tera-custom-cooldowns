using System.Collections.Generic;
using System.Linq;
using TCC.Data.Map;

namespace TCC.Data
{
    public class Dungeon : TSPropertyChanged
    {
        private string _shortName;
        private bool _doublesOnElite;
        private bool _show = false;
        private ItemLevelTier _requiredIlvl = ItemLevelTier.Tier0;
        private int _index = -1;
        public uint Id { get; }
        public string Name { get; }

        public string ShortName
        {
            get => _shortName;
            set
            {
                if (_shortName == value) return;
                _shortName = value;
                NPC();
            }
        }

        public short MaxBaseRuns { get; set; } = 1;

        public ItemLevelTier RequiredIlvl
        {
            get => _requiredIlvl;
            set
            {
                if (_requiredIlvl == value) return;
                _requiredIlvl = value;
                NPC();
                NPC(nameof(ItemLevel));
            }
        }

        public int ItemLevel => (int)RequiredIlvl;

        public bool Show
        {
            get => _show;
            set
            {
                if (_show == value) return;
                _show = value;
                NPC();
            }
        }

        public bool DoublesOnElite
        {
            get => _doublesOnElite;
            set
            {
                if (_doublesOnElite == value) return;
                _doublesOnElite = value;
                NPC();
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if(_index == value) return;
                _index = value;
                NPC();
            }
        }

        public int ActualRuns => MaxBaseRuns * (SessionManager.IsElite && DoublesOnElite ? 2 : 1);
        public string IconName { get; set; }
        public string Region => SessionManager.MapDatabase.GetDungeonGuardName(Id);

        public Dungeon(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
