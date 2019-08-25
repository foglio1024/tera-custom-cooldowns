using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Interop.JsonRPC;
using TCC.Parsing;
using FoglioUtils.Extensions;
using TCC.Settings;
using TCC.ViewModels;
using TeraPacketParser;
using TeraPacketParser.Data;

namespace TCC.Interop.Proxy
{
    public class ProxyMessageHandler
    {
        private readonly Dictionary<string, Delegate> Methods = new Dictionary<string, Delegate>
        {
            { "setUiMode", new Action<JObject>(SetUiMode) },
            { "setChatMode", new Action<JObject>(SetChatMode) },
            { "handleChatMessage", new Action<JObject>(HandleChatMessage) },
            { "handleRawPacket", new Action<JObject>(HandleRawPacket) },
        };

        private static void HandleRawPacket(JObject parameters)
        {
            //Log.CW("[ProxyMessageHandler] Handling raw packet");
            var direction = (MessageDirection)parameters["direction"].Value<uint>();
            var content = new Message(DateTime.UtcNow, direction, parameters["content"].Value<string>().Substring(4));
            PacketAnalyzer.EnqueuePacket(content);
        }
        private static void HandleChatMessage(JObject parameters)
        {
            var author = parameters["author"].Value<string>();
            var channel = parameters["channel"].Value<uint>();
            var message = parameters["message"].Value<string>().AddFontTagsIfMissing();
            if (author == "undefined") author = "System";

            if (!ChatWindowManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
                ChatWindowManager.Instance.CachePrivateMessage(channel, author, message);
            else
                ChatWindowManager.Instance.AddChatMessage(
                    new ChatMessage((ChatChannel)ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(x =>
                                        x.Id == channel && x.Joined).Index + 11, author, message));
        }
        private static void SetUiMode(JObject parameters)
        {
            //Log.CW("[ProxyMessageHandler] Setting UI mode");
            Session.InGameUiOn = parameters["uiMode"].Value<bool>();
        }
        private static void SetChatMode(JObject parameters)
        {
            //Log.CW("[ProxyMessageHandler] Setting chat mode");
            Session.InGameChatOpen = parameters["chatMode"].Value<bool>();
        }
        public void HandleResponse(Response res)
        {
        }
        public void HandleRequest(Request req)
        {
            //Log.CW($"[ProxyMessageHandler] Handling request for {req.Method}");
            if (!Methods.TryGetValue(req.Method, out var del)) return;
            del.DynamicInvoke(req.Parameters);
        }
    }
}