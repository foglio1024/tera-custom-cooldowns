using Newtonsoft.Json.Linq;
using Nostrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.Interop;

public static class Firebase
{
    readonly record struct Webhook(string URL, string AccountHash);
    
    static readonly List<Webhook> _registeredWebhooks = [];
    public static async void RegisterWebhook(string? url, bool online, string accountHash)
    {
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(accountHash)) return;
        var req = new JObject
        {
            {"webhook_hash", HashUtils.GenerateHash(url)},
            {"user", accountHash},
            {"online", online }
        };
        using var c = MiscUtils.GetDefaultHttpClient();
        c.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
        c.DefaultRequestHeaders.Add(HttpRequestHeader.AcceptCharset.ToString(), "utf-8");
        try
        {
            // todo: replace
            await c.PostAsync("http://us-central1-tcc-global-events.cloudfunctions.net/register_webhook",
                new StringContent(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())), Encoding.UTF8));
            if (online)
            {
                _registeredWebhooks.Add(new Webhook(url, accountHash));
            }
            else
            {
                var toRemove = _registeredWebhooks.FirstOrDefault(x => x.URL == url);
                if (toRemove == default) return;
                _registeredWebhooks.Remove(toRemove);
            }
        }
        catch
        {
            Log.F("Failed to register webhook.");
        }
    }
    public static async Task<bool> RequestWebhookExecution(string url, string accountHash)
    {
        bool canFire;
        var req = new JObject
        {
            { "webhook_hash" , HashUtils.GenerateHash(url)},
            { "user", accountHash }
        };
        using var c = MiscUtils.GetDefaultHttpClient();
        c.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
        c.DefaultRequestHeaders.Add(HttpRequestHeader.AcceptCharset.ToString(), "utf-8");
        try
        {
            // todo: replace
            var res = await c.PostAsync("http://us-central1-tcc-global-events.cloudfunctions.net/fire_webhook",
                new StringContent(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())), Encoding.UTF8));

            var jRes = JObject.Parse(await res.Content.ReadAsStringAsync());
            canFire = (jRes["canFire"] ?? throw new InvalidOperationException("Invalid webhook response format"))
                .Value<bool>();
            //Log.CW($"Webhook fire requested, result: {canFire}");
        }
        catch (WebException e)
        {
            Log.F($"Failed to request webhook execution. Webhook will be executed anyway: {e}");
            canFire = true;
        }
        catch (InvalidOperationException e)
        {
            Log.F($"Failed to request webhook execution. Webhook will be executed anyway: {e}");
            canFire = true;
        }
        catch (Exception e)
        {
            Log.F($"Failed to request webhook execution. Webhook will be executed anyway: {e}");
            canFire = true;
        }

        return canFire;
    }

    public static void Dispose()
    {
        try
        {
            var webhooks = new List<Webhook> { Capacity = _registeredWebhooks.Count }.ToArray();
            _registeredWebhooks.CopyTo(webhooks);
            webhooks.ToList().ForEach(w =>
            {
                RegisterWebhook(w.URL, false, w.AccountHash);
            });
        }
        catch (Exception e)
        {
            Log.F($"Failed to dispose Firebase webhooks: {e}");
        }
    }
}