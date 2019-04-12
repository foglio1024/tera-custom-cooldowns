using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TCC.Interop
{
    public static class Firebase
    {
        public static async void RegisterWebhook(string webhook, bool online)
        {
            if (string.IsNullOrEmpty(webhook)) return;
            if (string.IsNullOrEmpty(SessionManager.CurrentAccountName)) return;
            var req = new JObject
            {
                {"webhook", Utils.GenerateHash(webhook)},
                {"user", Utils.GenerateHash(SessionManager.CurrentAccountName)},
                {"online", online }
            };
            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {
                    var res = await c.UploadStringTaskAsync(
                        new Uri("http://us-central1-tcc-global-events.cloudfunctions.net/register_webhook"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));
                    Log.All("Webhook registered");
                }
                catch
                {
                    Log.F($"Failed to register webhook.");
                }
            }
        }

        public static async Task<bool> RequestWebhookExecution(string webhook)
        {
            var canFire = true;
            var req = new JObject
            {
                { "webhook" , Utils.GenerateHash(webhook)},
                { "user", Utils.GenerateHash(SessionManager.CurrentAccountName) }
            };
            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {

                    var res = await c.UploadStringTaskAsync(
                        new Uri("http://us-central1-tcc-global-events.cloudfunctions.net/fire_webhook"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));

                    var jRes = JObject.Parse(res);
                    canFire = jRes["canFire"].Value<bool>();
                    Log.All($"Webhook fire requested, result: {canFire}");

                }
                catch (WebException e)
                {
                    Log.F($"Failed to request webhook execution. Webhook will be executed anyway: {e}");
                    canFire = true;
                }

            }

            return canFire;
        }
    }
}