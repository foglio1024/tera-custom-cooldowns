using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TCC.Data;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class CivilUnrestGuild : TSPropertyChanged
    {
        private float _towerHp;
        private uint _towersDestroyed;
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                NPC();
            }
        }

        public uint Id { get; set; }
        public float TowerHp
        {
            get => _towerHp;
            set
            {
                if (_towerHp == value) return;
                _towerHp = value;
                NPC();
            }
        }
        public uint TowersDestroyed
        {
            get => _towersDestroyed;
            set
            {
                if (_towersDestroyed == value) return;
                _towersDestroyed = value;
                NPC();
            }
        }

        public CivilUnrestGuild()
        {
        }
    }
    public class CivilUnrestViewModel : TccWindowViewModel
    {
        public event Action Teleported;
        private SynchronizedObservableCollection<CivilUnrestGuild> _guilds;
        private bool _cuZone;

        public ICollectionViewLiveShaping Guilds
        {
            get
            {
                var ret = Utils.InitLiveView(p => p != null, _guilds, new string[] { },
                    new[] { new SortDescription(nameof(CivilUnrestGuild.TowersDestroyed), ListSortDirection.Descending) });
                return ret;
            }
        }

        public bool CuZone
        {
            get => _cuZone;
            set
            {
                if (_cuZone == value) return;
                _cuZone = value;
                _dispatcher.Invoke(Teleported);
            }
        }

        public CivilUnrestViewModel()
        {
            _dispatcher = App.BaseDispatcher;
            _guilds = new SynchronizedObservableCollection<CivilUnrestGuild>();
        }

        public void AddGuild(CityWarGuildInfo guildInfo)
        {
            CivilUnrestGuild g = null;
            g = _guilds.FirstOrDefault(x => x.Id == guildInfo.Id);
            if (g != null)
            {
                g.TowerHp = guildInfo.TowerHp;
                //TODO: add kills and deaths?
            }
            else
            {
                _guilds.Add(new CivilUnrestGuild() { Id = guildInfo.Id, Name = "", TowerHp = guildInfo.TowerHp, TowersDestroyed = 0 });
            }
        }

        public void SetGuildName(uint id, string name)
        {
            CivilUnrestGuild g = null;
            g = _guilds.FirstOrDefault(x => x.Id == id);
            if (g != null)
            {
                g.Name = name;
            }
        }

        public void AddDestroyedGuildTower(uint id)
        {
            CivilUnrestGuild g = null;
            g = _guilds.FirstOrDefault(x => x.Id == id);
            if (g != null)
            {
                g.TowersDestroyed++;
            }
        }
    }
}