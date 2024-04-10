using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nostrum;

namespace TCC.Interop.Moongourd;

public class KabedonManager : IMoongourdManager
{
    private const string SEARCH_URL = "https://moongourd.com/api/mg/search?";
    private const string LOG_URL = "https://storage.googleapis.com/mg-uploads/#region#/#zoneId#/#bossId#/1/#logId#.json";
    private const int MAX_ENCOUNTERS = 5;

    private bool _requestInProgress;

    public event Action? Started;

    public event Action<List<IMoongourdEncounter>>? Finished;

    public event Action<string>? Failed;

    private static string BuildSearchUrl(string region, string server, string name)
    {
        return $"{SEARCH_URL}region={region}&name={name}&server={server}";
    }

    private static string BuildLogUrl(string region, int zoneId, int bossId, long logId)
    {
        return LOG_URL.Replace("#region#", region)
            .Replace("#zoneId#", zoneId.ToString())
            .Replace("#bossId#", bossId.ToString())
            .Replace("#logId#", logId.ToString());
    }

    public void GetEncounters(string playerName, string region, string playerServer = "", int areaId = 0, int bossId = 0)
    {
        Task.Run(async () =>
        {
            if (_requestInProgress) return;
            Started?.Invoke();
            _requestInProgress = true;
            var results = new List<IMoongourdEncounter>();
            using var webClient = MiscUtils.GetDefaultHttpClient();

            try
            {
                var strResp = await webClient.GetStringAsync(BuildSearchUrl(region, playerServer, playerName));
                var jResp = JArray.Parse(strResp)[1]; //[0] is { count: 0 }, [1] is log array

                var count = 0;

                foreach (var jEntry in jResp)
                {
                    if (count >= MAX_ENCOUNTERS) break;
                    if (jEntry["playerName"]!.Value<string>() != playerName) continue;
                    var logId = long.Parse(jEntry["logId"]!.Value<string>()!);
                    var logZoneId = jEntry["zoneId"]!.Value<int>();
                    var logBossId = jEntry["bossId"]!.Value<int>();

                    var logUrl = BuildLogUrl(region, logZoneId, logBossId, logId);

                    var strLog = await webClient.GetStringAsync(logUrl);
                    var jLog = JObject.Parse(strLog);

                    var dps = int.Parse(jLog["members"]!.FirstOrDefault(y => y["playerName"]!.Value<string>() == playerName)!["playerDps"]!.Value<string>()!);
                    var deaths = int.Parse(jLog["members"]!.FirstOrDefault(y => y["playerName"]!.Value<string>() == playerName)!["playerDeaths"]!.Value<string>()!);

                    var encounter = new MoongourdEncounter
                    {
                        PlayerName = playerName,
                        AreaId = logZoneId,
                        BossId = logBossId,
                        LogId = logId,
                        PlayerDps = dps,
                        PlayerDeaths = deaths
                    };

                    results.Add(encounter);

                    count++;
                }
            }
            catch (Exception e)
            {
                //Log.CW(e.ToString());
                Failed?.Invoke(e.ToString());
                _requestInProgress = false;
                return;
            }

            Finished?.Invoke(results);
            _requestInProgress = false;
        });
    }
}