using Newtonsoft.Json.Linq;
using Nostrum.Extensions;
using System;
using System.Collections.Generic;
using TCC.Utils;
using TeraPacketParser;
using TeraPacketParser.Data;
using TeraPacketParser.Sniffing.JsonRPC;
using Log = TCC.Utils.Log;

namespace TCC.Interop.Proxy
{
    /// <summary>
    /// Handles requests coming from tcc-stub.
    /// </summary>
    public class StubMessageParser
    {
        private readonly Dictionary<string, Delegate> Methods = new()
        {
            { "setUiMode", new Action<JObject>(SetUiMode) },
            { "setChatMode", new Action<JObject>(SetChatMode) },
            { "handleChatMessage", new Action<JObject>(HandleChatMessage) },
            { "handleRawPacket", new Action<JObject>(HandleRawPacket) },
            { "enqueueNotification", new Action<JObject>(HandleEnqueueNotification) },
        };

        public static event Action<bool>? SetUiModeEvent;
        public static event Action<bool>? SetChatModeEvent;
        public static event Action<string, uint, string>? HandleChatMessageEvent;
        public static event Action<Message>? HandleRawPacketEvent;

        private static void SetUiMode(JObject parameters)
        {
            var jMode = parameters["uiMode"];
            if (jMode == null) return;
            SetUiModeEvent?.Invoke(jMode.Value<bool>());
            //Game.InGameUiOn = jMode.Value<bool>();
        }
        private static void SetChatMode(JObject parameters)
        {
            var jMode = parameters["chatMode"];
            if (jMode == null) return;
            SetChatModeEvent?.Invoke(jMode.Value<bool>());

            //Game.InGameChatOpen = jMode.Value<bool>();
        }
        private static void HandleChatMessage(JObject parameters)
        {
            var jAuthor = parameters["author"];
            if (jAuthor == null) return;
            var author = jAuthor.Value<string>()!;
            if (author == "undefined") author = "System";

            var jChannel = parameters["channel"];
            if (jChannel == null) return;
            var channel = jChannel.Value<uint>();

            var jMessage = parameters["message"];
            if (jMessage == null) return;
            var message = jMessage.Value<string>().AddFontTagsIfMissing();

            HandleChatMessageEvent?.Invoke(author, channel, message);

            //if (!ChatManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
            //    ChatManager.Instance.CachePrivateMessage(channel, author, message);
            //else
            //    ChatManager.Instance.AddChatMessage(
            //        ChatManager.Instance.Factory.CreateMessage((ChatChannel)ChatManager.Instance.PrivateChannels.FirstOrDefault(x =>
            //            x.Id == channel && x.Joined).Index + 11, author, message));


        }
        private static void HandleRawPacket(JObject parameters)
        {
            var jDir = parameters["direction"];
            if (jDir == null) return;

            var jContent = parameters["content"];
            if (jContent == null) return;

            var direction = (MessageDirection)jDir.Value<uint>();
            var content = new Message(DateTime.UtcNow, direction, jContent.Value<string>()![4..]);

            HandleRawPacketEvent?.Invoke(content);

            //PacketAnalyzer.EnqueuePacket(content);
        }
        private static void HandleEnqueueNotification(JObject parameters)
        {
            var jTitle = parameters["title"];
            var message = parameters["message"]?.Value<string>();
            if (message == null) return;
            var jNotificationType = parameters["notificationType"];
            var jSecDuration = parameters["secDuration"];

            var title = jTitle?.Value<string>() ?? "TCC";
            var type = NotificationType.None;
            try
            {
                type = (NotificationType)(jNotificationType?.Value<int>() ?? 0);
            }
            catch
            {
                // ignored, just too lazy to check
            }

            var duration = jSecDuration?.Value<int>() ?? -1;

            Log.N(title, message, type, duration);
        }

        public void HandleRequest(Request req)
        {
            if (!Methods.TryGetValue(req.Method, out var del)) return;
            del.DynamicInvoke(req.Parameters);
        }
        public void HandleResponse(Response res)
        {
        }
    }
}