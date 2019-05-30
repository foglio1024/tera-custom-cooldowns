using System;
using Newtonsoft.Json.Linq;
using TCC.Data;
using TeraDataLite;

namespace TCC.Moongourd
{
    public class MoongourdEncounter
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string PlayerName { get; }
        public Class PlayerClass { get; }
        public string PlayerServer { get; }
        public string LogId { get; set; }
        public int Timestamp { get; set; }
        public int PlayerDps { get; set; }
        public int PartyDps { get; set; }
        public int PlayerDeaths { get; set; }
        public int AreaId { get; set; }
        public int BossId { get; set; }
        public string DungeonName { get; set; }
        public string BossName { get; set; }
        public BrowseCommand Browse { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        public MoongourdEncounter(JObject jEncounter)
        {
            Browse = new BrowseCommand(this);
            PlayerName = jEncounter["playerName"].Value<string>();
            Enum.TryParse<Class>(jEncounter["playerClass"].Value<string>(), out var cl);
            PlayerClass = cl;
            PlayerServer = jEncounter["playerServer"].Value<string>();
            LogId = jEncounter["logId"].Value<string>();
            Timestamp = jEncounter["timestamp"].Value<int>();
            PlayerDps = jEncounter["playerDps"].Value<int>();
            PartyDps = jEncounter["partyDps"].Value<int>();
            try
            {
                PlayerDeaths = jEncounter["playerDeaths"].Value<int>();
            }
            catch (Exception)
            {
                PlayerDeaths = -1;
            }

            DungeonName = jEncounter["dungeonName"].Value<string>();
            BossName = jEncounter["bossName"].Value<string>();
            BossId = jEncounter["bossId"].Value<int>();
            AreaId = jEncounter["areaId"].Value<int>();
        }

/*
        public MoongourdEncounter()
        {
            var r = new Random();
            PlayerName = "Player";
            PlayerClass = Class.Warrior;
            PlayerServer = "Mystel";
            LogId = "123456789123";
            Timestamp = 1234567891;
            PlayerDps = r.Next(100000, 10000000);
            PlayerDeaths = r.Next(0, 10);
            PartyDps = 10000000;
            DungeonName = "Sirjuka Gallery";
            BossName = "Barkud";
        }
*/
    }
}