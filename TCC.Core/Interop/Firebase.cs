using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FoglioUtils;
using Newtonsoft.Json.Linq;
using TCC.Interop.Proxy;
using TCC.Settings;
using FoglioUtils.Extensions;

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
                {"webhook", HashUtils.GenerateHash(webhook)},
                {"user", HashUtils.GenerateHash(SessionManager.CurrentAccountName)},
                {"online", online }
            };
            using (var c = FoglioUtils.MiscUtils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;
                try
                {
                    await c.UploadStringTaskAsync(
                        new Uri("http://us-central1-tcc-global-events.cloudfunctions.net/register_webhook"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(req.ToString())));
                }
                catch
                {
                    Log.F($"Failed to register webhook.");
                }
            }
        }
        public static async Task<bool> RequestWebhookExecution(string webhook)
        {
            bool canFire;
            var req = new JObject
            {
                { "webhook" , HashUtils.GenerateHash(webhook)},
                { "user", HashUtils.GenerateHash(SessionManager.CurrentAccountName) }
            };
            using (var c = FoglioUtils.MiscUtils.GetDefaultWebClient())
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

        public static async void SendUsageStatAsync()
        {
            if (SettingsHolder.StatSentVersion == App.AppVersion &&
                SettingsHolder.StatSentTime.Month == DateTime.UtcNow.Month &&
                SettingsHolder.StatSentTime.Day == DateTime.UtcNow.Day) return;

            try
            {
                using (var c = FoglioUtils.MiscUtils.GetDefaultWebClient())
                {
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    c.Encoding = Encoding.UTF8;

                    var accountNameHash = SHA256.Create().ComputeHash(SessionManager.CurrentAccountName.ToByteArray())
                        .ToStringEx();
                    var js = new JObject
                    {
                        {"region", SessionManager.Server.Region},
                        {"server", SessionManager.Server.ServerId},
                        {"account", accountNameHash},
                        {"tcc_version", App.AppVersion},
                        {
                            "updated", SettingsHolder.StatSentTime.Month == DateTime.Now.Month &&
                                       SettingsHolder.StatSentTime.Day == DateTime.Now.Day &&
                                       SettingsHolder.StatSentVersion != App.AppVersion
                        },
                        {
                            "settings_summary", new JObject
                            {
                                {
                                    "windows", new JObject
                                    {
                                        {"cooldown", SettingsHolder.CooldownWindowSettings.Enabled},
                                        {"buffs", SettingsHolder.BuffWindowSettings.Enabled},
                                        {"character", SettingsHolder.CharacterWindowSettings.Enabled},
                                        {"class", SettingsHolder.ClassWindowSettings.Enabled},
                                        {"chat", SettingsHolder.ChatEnabled},
                                        {"group", SettingsHolder.GroupWindowSettings.Enabled}
                                    }
                                },
                                {
                                    "generic", new JObject
                                    {
                                        {"proxy_enabled", ProxyInterface.Instance.IsStubAvailable}
                                    }
                                }
                            }
                        }
                    };

                    await c.UploadStringTaskAsync(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/usage_stat"),
                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));

                    SettingsHolder.StatSentTime = DateTime.UtcNow;
                    SettingsHolder.StatSentVersion = App.AppVersion;
                }
            }
            catch
            {
                //TODO: write error?
            }
        }

    }
}