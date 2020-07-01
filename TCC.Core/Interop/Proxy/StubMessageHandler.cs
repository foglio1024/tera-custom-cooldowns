using Newtonsoft.Json.Linq;
using Nostrum.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Analysis;
using TCC.Data;
using TCC.Interop.JsonRPC;
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
            var jDir = parameters["direction"];
            if (jDir == null) return;

            var jContent = parameters["content"];
            if (jContent == null) return;

            var direction = (MessageDirection)jDir.Value<uint>();
            var content = new Message(DateTime.UtcNow, direction, jContent.Value<string>().Substring(4));
            PacketAnalyzer.EnqueuePacket(content);
        }
        private static void HandleChatMessage(JObject parameters)
        {
            var jAuthor = parameters["author"];
            if (jAuthor == null) return;
            var author = jAuthor.Value<string>();
            if (author == "undefined") author = "System";

            var jChannel = parameters["channel"];
            if (jChannel == null) return;
            var channel = jChannel.Value<uint>();

            var jMessage = parameters["message"];
            if (jMessage == null) return;
            var message = jMessage.Value<string>().AddFontTagsIfMissing();

            if (!ChatManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
                ChatManager.Instance.CachePrivateMessage(channel, author, message);
            else
                ChatManager.Instance.AddChatMessage(
                    ChatManager.Instance.Factory.CreateMessage((ChatChannel)ChatManager.Instance.PrivateChannels.FirstOrDefault(x =>
                                        x.Id == channel && x.Joined).Index + 11, author, message));
        }
        private static void SetUiMode(JObject parameters)
        {
            var jMode = parameters["uiMode"];
            if (jMode == null) return;
            Game.InGameUiOn = jMode.Value<bool>();
        }
        private static void SetChatMode(JObject parameters)
        {
            var jMode = parameters["chatMode"];
            if (jMode == null) return;
            Game.InGameChatOpen = jMode.Value<bool>();
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