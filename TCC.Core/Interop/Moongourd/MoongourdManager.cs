using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Interop.Moongourd
{
    [Obsolete]
    public class MoongourdManager : IMoongourdManager
    {
        private const string RecentUploadsUrl = "https://moongourd.com/api/bot/recent_uploads_tcc";
        private bool _asking;
        private static JArray? LastResponse { get; set; } = new JArray();

        public event Action<List<MoongourdEncounter>>? Finished;

        public event Action? Started;

        public event Action<string>? Failed;

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
            using var c = new WebClient();
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
                    // ReSharper disable once AccessToDisposedClosure
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
                        Finished?.Invoke(ret);
                    }
                    catch
                    {
                        LastResponse = null;
                    }
                }
                catch (Exception e)
                {
                    Failed?.Invoke(e.ToString());
                    // ignored
                }

                _asking = false;
            }).Start();
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
}