using Nostrum;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.Interop
{
    public static class Firebase
    {
        private static readonly List<Tuple<string, string>> _registeredWebhooks = new();
        public static async void RegisterWebhook(string? webhook, bool online, string accountHash)
        {
            if (string.IsNullOrEmpty(webhook)) return;
            if (string.IsNullOrEmpty(accountHash)) return;
            var req = new JObject
            {
                {"webhook", HashUtils.GenerateHash(webhook)},
                {"user", accountHash},
                {"online", online }
            };
            using var c = MiscUtils.GetDefaultWebClient();
            c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
            c.Encoding = Encoding.UTF8;
            try
            {
                await c.UploadStringTaskAsync(
                    new Uri("http://us-central1-tcc-global-events.cloudfunctions.net/register_webhook"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));
                if (online)
                {
                    _registeredWebhooks.Add(new(webhook, accountHash));
                }
                else
                {
                    var toRemove = _registeredWebhooks.FirstOrDefault(x => x.Item1 == webhook);
                    if (toRemove == null) return;
                    _registeredWebhooks.Remove(toRemove);
                }
            }
            catch
            {
                Log.F($"Failed to register webhook.");
            }
        }
        public static async Task<bool> RequestWebhookExecution(string webhook, string accountHash)
        {
            bool canFire;
            var req = new JObject
            {
                { "webhook" , HashUtils.GenerateHash(webhook)},
                { "user", accountHash }
            };
            using var c = MiscUtils.GetDefaultWebClient();
            c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
            c.Encoding = Encoding.UTF8;
            try
            {
                var res = await c.UploadStringTaskAsync(
                    new Uri("http://us-central1-tcc-global-events.cloudfunctions.net/fire_webhook"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));

                var jRes = JObject.Parse(res);
                canFire = (jRes["canFire"] ?? throw new InvalidOperationException("Invalid webhook response format"))
                    .Value<bool>();
                Log.CW($"Webhook fire requested, result: {canFire}");

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

            return canFire;
        }

        public static async Task<bool> SendUsageStatAsync(JObject js)
        {

            try
            {
                using var c = MiscUtils.GetDefaultWebClient();
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;


                await c.UploadStringTaskAsync(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/usage_stat"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));

                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Dispose()
        {
            try
            {
                var webhooks = new List<Tuple<string, string>> () { Capacity = _registeredWebhooks.Count}.ToArray();
                _registeredWebhooks.CopyTo(webhooks);
                webhooks.ToList().ForEach(w =>
                {
                    RegisterWebhook(w.Item1, false, w.Item2);
                });
            }
            catch (Exception e)
            {
                Log.F($"Failed to dispose Firebase webhooks: {e}");
            }
        }
    }
}