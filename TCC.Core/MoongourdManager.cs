using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using TCC.Data;

namespace TCC
{
    public class MoongourdManager
    {
        private const string RecentUploadsUrl = "https://moongourd.com/api/bot/recent_uploads_tcc\r\n";
        private bool _asking;
        private static JObject LastRequest { get; set; }
        private static JArray LastResponse { get; set; } = new JArray();
        public event Action<List<MoongourdEncounter>> Done;
        public event Action Started;

        private JObject BuildRequest(string playerName, string region, string playerServer = "", int areaId = 0, int bossId = 0)
        {
            var jPlayerName = new JProperty("playerName", playerName);
            var jRegion = new JProperty("region", region);
            var jPlayerServer = new JProperty("playerServer", playerServer);
            var jAreaId = new JProperty("areaId", areaId);
            var jBossId = new JProperty("bossId", bossId);

            var req = new JObject { jPlayerName, jRegion };
            if (playerServer != "") req.Add(jPlayerServer);
            if (areaId != 0) req.Add(jAreaId);
            if (bossId != 0) req.Add(jBossId);

            return req;
        }
        private void DownloadEncountersJson(JObject request)
        {
            if (request == null) return;
            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                new Task(() =>
                {
                    if (_asking) return;
                    Started?.Invoke();
                    _asking = true;
                    try
                    {
                        var str = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(request.ToString()));
                        var resp = c.UploadString(new Uri(RecentUploadsUrl), str);
                        LastResponse = null;
                        try
                        {
                            LastResponse = JArray.Parse(resp);
                            var ret = new List<MoongourdEncounter>();
                            if (LastResponse == null) return;
                            foreach (var jToken in LastResponse)
                            {
                                var jEncounter = (JObject)jToken;
                                var encounter = new MoongourdEncounter(jEncounter);
                                ret.Add(encounter);
                            }
                            Done?.Invoke(ret);
                        }
                        catch (Exception e)
                        {
                            LastResponse = null;
                        }
                    }
                    catch (Exception e) { }
                    _asking = false;
                }).Start();
            }

        }

        public void GetEncounters(string playerName, string region, string playerServer = "", int areaId = 0, int bossId = 0)
        {
            var req = BuildRequest(playerName, region, playerServer, areaId, bossId);
            //if (req.ToString() != LastRequest?.ToString()) DownloadEncountersJson(req);
            DownloadEncountersJson(req);
            //var ret = new List<MoongourdEncounter>();
            //if (LastResponse == null) return;
            //foreach (var jToken in LastResponse)
            //{
            //    var jEncounter = (JObject)jToken;
            //    var encounter = new MoongourdEncounter(jEncounter);
            //    ret.Add(encounter);
            //}
            //return ret;
        }

    }

    public class MoongourdEncounter
    {
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
    }
    public class BrowseCommand : ICommand
    {
        private MoongourdEncounter _encounter;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var reg = SettingsManager.LastRegion.ToLower();
            reg = reg.StartsWith("eu") ? "eu" : reg;
            reg = reg == "na" ? "" : reg;
            reg = reg == "" ? reg : reg + "/";
            Process.Start($"https://" + $"moongourd.com/{reg}encounter?area={_encounter.AreaId}&boss={_encounter.BossId}&log={_encounter.LogId}");
        }

        public BrowseCommand(MoongourdEncounter enc)
        {
            _encounter = enc;
        }
        public event EventHandler CanExecuteChanged;
    }
}
