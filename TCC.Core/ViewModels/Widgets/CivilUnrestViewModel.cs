using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using FoglioUtils;
using TCC.Data;
using TCC.Parsing;
using TCC.Settings;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
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
                N();
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
                N();
            }
        }
        public uint TowersDestroyed
        {
            get => _towersDestroyed;
            set
            {
                if (_towersDestroyed == value) return;
                _towersDestroyed = value;
                N();
            }
        }
    }

    [TccModule]
    public class CivilUnrestViewModel : TccWindowViewModel
    {
        public bool CivilUnrest => Session.CivilUnrestZone;
        private readonly SynchronizedObservableCollection<CivilUnrestGuild> _guilds;

        public ICollectionViewLiveShaping Guilds
        {
            get
            {
                var ret = CollectionViewUtils.InitLiveView(p => p != null, _guilds, new string[] { },
                    new[]
                    {
                        //new SortDescription(nameof(CivilUnrestGuild.TowerHp), ListSortDirection.Descending),
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
            for (var i = 0; i < limit; i++)
            {
                var item =
                    WindowManager.CivilUnrestWindow.GuildList.Items[i] as CivilUnrestGuild;
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

        public CivilUnrestViewModel(WindowSettings settings) : base(settings)
        {
            _guilds = new SynchronizedObservableCollection<CivilUnrestGuild>();
        }

        protected override void InstallHooks()
        {
            Session.Teleported += NotifyTeleported;
            PacketAnalyzer.NewProcessor.Hook<S_REQUEST_CITY_WAR_MAP_INFO_DETAIL>(OnRequestCityWarMapInfoDetail);
            PacketAnalyzer.NewProcessor.Hook<S_REQUEST_CITY_WAR_MAP_INFO>(OnRequestCityWarMapInfo);
            PacketAnalyzer.NewProcessor.Hook<S_DESTROY_GUILD_TOWER>(OnDestroyGuildTower);
        }

        protected override void RemoveHooks()
        {
            Session.Teleported -= NotifyTeleported;
            PacketAnalyzer.NewProcessor.Unhook<S_REQUEST_CITY_WAR_MAP_INFO_DETAIL>(OnRequestCityWarMapInfoDetail);
            PacketAnalyzer.NewProcessor.Unhook<S_REQUEST_CITY_WAR_MAP_INFO>(OnRequestCityWarMapInfo);
            PacketAnalyzer.NewProcessor.Unhook<S_DESTROY_GUILD_TOWER>(OnDestroyGuildTower);
        }

        private void OnDestroyGuildTower(S_DESTROY_GUILD_TOWER m)
        {
            try
            {
                AddDestroyedGuildTower(m.SourceGuildId);
            }
            catch
            {
                // ignored
            }
        }
        private void OnRequestCityWarMapInfo(S_REQUEST_CITY_WAR_MAP_INFO m)
        {
            try
            {
                m.Guilds.ToList().ForEach(x => WindowManager.ViewModels.CivilUnrest.AddGuild(x));
            }
            catch
            {
                // ignored
            }
        }
        private void OnRequestCityWarMapInfoDetail(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL m)
        {
            try
            {
                m.GuildDetails.ToList().ForEach(x => WindowManager.ViewModels.CivilUnrest.SetGuildName(x.Item1, x.Item2));
            }
            catch
            {
                // ignored
            }
        }

        public void AddGuild(CityWarGuildData guildInfo)
        {
            var g = _guilds.FirstOrDefault(x => x.Id == guildInfo.Id);
            if (g != null)
            {
                g.TowerHp = guildInfo.TowerHp;
                if (g.Name != "") return;
                if(guildInfo.Self) g.Name = WindowManager.ViewModels.Dashboard.CurrentCharacter?.GuildName;
                //TODO: add kills and deaths?
            }
            else
            {
                var name = "";
                if (guildInfo.Self) name = WindowManager.ViewModels.Dashboard.CurrentCharacter?.GuildName;
                _guilds.Add(new CivilUnrestGuild() { Id = guildInfo.Id, Name = name, TowerHp = guildInfo.TowerHp, TowersDestroyed = 0 });
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
        private void NotifyTeleported()
        {
            N(nameof(CivilUnrest));
        }
    }
}