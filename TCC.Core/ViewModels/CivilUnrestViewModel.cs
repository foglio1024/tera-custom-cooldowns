using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
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
    }
    public class CivilUnrestViewModel : TccWindowViewModel
    {
        public event Action Teleported;
        private readonly SynchronizedObservableCollection<CivilUnrestGuild> _guilds;

        public ICollectionViewLiveShaping Guilds
        {
            get
            {
                var ret = Utils.InitLiveView(p => p != null, _guilds, new string[] { },
                    new[]
                    {
                        new SortDescription(nameof(CivilUnrestGuild.TowerHp), ListSortDirection.Descending),
                        new SortDescription(nameof(CivilUnrestGuild.TowersDestroyed), ListSortDirection.Descending)
                    });
                return ret;
            }
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder("Civil Unrest temporary rank (top 5):\\");
            var limit = 5;
            limit = _guilds.Count > limit ? limit : _guilds.Count;
            for (int i = 0; i < limit; i++)
            {
                var item =
                    ((WindowManager.CivilUnrestWindow.GuildList.Items[i])) as CivilUnrestGuild;
                sb.Append(item?.Name);
                sb.Append(" | ");
                sb.Append($"HP: {item?.TowerHp:##0%}");
                sb.Append(" | ");
                sb.Append($"Towers: {item?.TowersDestroyed}");
                sb.Append("\\");
            }
            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch
            {
                WindowManager.FloatingButton.NotifyExtended("Boss window", "Failed to copy boss HP to clipboard.", NotificationType.Error);
                ChatWindowManager.Instance.AddTccMessage("Failed to copy boss HP.");
            }
        }

        public CivilUnrestViewModel()
        {
            Dispatcher = App.BaseDispatcher;
            _guilds = new SynchronizedObservableCollection<CivilUnrestGuild>();
        }

        public void AddGuild(CityWarGuildInfo guildInfo)
        {
            var g = _guilds.FirstOrDefault(x => x.Id == guildInfo.Id);
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
            var g = _guilds.FirstOrDefault(x => x.Id == id);
            if (g != null)
            {
                g.Name = name;
            }
        }

        public void AddDestroyedGuildTower(uint id)
        {
            var g = _guilds.FirstOrDefault(x => x.Id == id);
            if (g != null)
            {
                g.TowersDestroyed++;
            }
        }

        public void NotifyTeleported()
        {
            Dispatcher.Invoke(Teleported);
        }
    }
}