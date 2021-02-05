using Newtonsoft.Json.Linq;
using Nostrum;
using System;
using System.Windows.Input;
using TCC.Utilities;
using TeraDataLite;

namespace TCC.Interop.Moongourd
{
    public class MoongourdEncounter
    {
        public string PlayerName { get; set; } = "";
        public long LogId { get; set; }
        public int PlayerDps { get; set; }
        public int PlayerDeaths { get; set; }
        public int AreaId { get; set; }
        public int BossId { get; set; }
        public string DungeonName => Game.DB.RegionsDatabase.GetZoneName((uint)AreaId);
        public string BossName => Game.DB.MonsterDatabase.GetMonsterName((uint)BossId, (uint)AreaId);
        public ICommand Browse { get; }
        //public Class PlayerClass { get; }
        //public string PlayerServer { get; } = "";
        //public int Timestamp { get; set; }
        //public int PartyDps { get; set; }

        public MoongourdEncounter()
        {
            Browse = new RelayCommand(_ =>
            {
                var reg = App.Settings.LastLanguage.ToLower();
                reg = reg.StartsWith("eu") ? "eu" : reg;
                reg = reg == "na" ? "" : reg + "/";
                TccUtils.OpenUrl("https://" + $"moongourd.com/{reg}upload/{AreaId}/{BossId}/1/{LogId}");
            });
        }

        [Obsolete]
        public MoongourdEncounter(JObject jEncounter) : this()
        {
            PlayerName = jEncounter["playerName"]!.Value<string>();
            Enum.TryParse<Class>(jEncounter["playerClass"]!.Value<string>(), out var cl);
            //PlayerClass = cl;
            //PlayerServer = jEncounter["playerServer"]!.Value<string>();
            LogId = long.Parse(jEncounter["logId"]!.Value<string>());
            //Timestamp = jEncounter["timestamp"]!.Value<int>();
            PlayerDps = jEncounter["playerDps"]!.Value<int>();
            //PartyDps = jEncounter["partyDps"]!.Value<int>();
            try
            {
                PlayerDeaths = jEncounter["playerDeaths"]!.Value<int>();
            }
            catch (Exception)
            {
                PlayerDeaths = -1;
            }

            //DungeonName = jEncounter["dungeonName"]!.Value<string>();
            //BossName = jEncounter["bossName"]!.Value<string>();
            BossId = jEncounter["bossId"]!.Value<int>();
            AreaId = jEncounter["areaId"]!.Value<int>();
        }
    }
}