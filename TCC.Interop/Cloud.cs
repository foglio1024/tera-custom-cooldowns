using Nostrum;
using SharpPcap;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace TCC.Interop;

public static class Cloud
{
    readonly record struct UsageStat(string Region,
        uint ServerId,
        string AccountIdHash,
        string TccVersion,
        string BinaryHash,
        bool IsDailyFirst
    );

    public static async Task<bool> SendUsageStatAsync(string region, uint server, string account, string version, bool isDailyFirst)
    {
        try
        {
            using var c = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, "https://foglio.ns0.it/tcc/usage-stats/post")
            {
                Content = JsonContent.Create(new UsageStat(region, server, account, version, HashUtils.GenerateFileHash(Assembly.GetEntryAssembly()?.Location ?? ""), isDailyFirst)),
            };
            req.Headers.Add("User-Agent", "TCC/Windows");
            var resp = await c.SendAsync(req);

            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

}