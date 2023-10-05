using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Nostrum;
using TCC.Utils;

namespace TCC.Interop;

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
            using var client = MiscUtils.GetDefaultHttpClient();
            client.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
            await client.PostAsync(webhook, new StringContent(msg.ToString(), Encoding.UTF8));
        }
        catch (Exception e)
        {
            Log.N("TCC Discord notifier", "Failed to send Discord notification.", NotificationType.Error);
            Log.F($"Failed to execute webhook: {e}");
        }

    }
}