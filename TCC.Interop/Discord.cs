using System;
using System.Net;
using System.Text;
using Nostrum;
using Newtonsoft.Json.Linq;
using TCC.Utils;

namespace TCC.Interop
{
    public static class Discord
    {
        public static async void FireWebhook(string webhook, string message, string usernameOverride, string accountHash)
        {
            if (!await Firebase.RequestWebhookExecution(webhook, accountHash)) return;
            var msg = new JObject
            {
                {"content", message},
                {"username", usernameOverride},
                {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
            };
            

            try
            {
                using var client = MiscUtils.GetDefaultWebClient();
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.UploadString(webhook, "POST", msg.ToString());
            }
            catch (Exception e)
            {
                Log.N("TCC Discord notifier", "Failed to send Discord notification.", NotificationType.Error);
                Log.F($"Failed to execute webhook: {e}");
            }

        }
    }
}
