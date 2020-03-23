using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TCC.Data;
using TCC.Interop.JsonRPC;
using TCC.Analysis;
using Nostrum.Extensions;
using TCC.ViewModels;
using TeraPacketParser;
using TeraPacketParser.Data;

namespace TCC.Interop.Proxy
{
    /// <summary>
    /// Handles requests coming from tcc-stub.
    /// </summary>
    public class StubMessageHandler
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

            if (!ChatManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
                ChatManager.Instance.CachePrivateMessage(channel, author, message);
            else
                ChatManager.Instance.AddChatMessage(
                    ChatManager.Instance.Factory.CreateMessage((ChatChannel)ChatManager.Instance.PrivateChannels.FirstOrDefault(x =>
                                        x.Id == channel && x.Joined).Index + 11, author, message));
        }
        private static void SetUiMode(JObject parameters)
        {
            Game.InGameUiOn = parameters["uiMode"].Value<bool>();
        }
        private static void SetChatMode(JObject parameters)
        {
            Game.InGameChatOpen = parameters["chatMode"].Value<bool>();
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