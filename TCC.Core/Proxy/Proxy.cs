using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Parsing;
using TCC.TeraCommon;
using TCC.ViewModels;

namespace TCC.Proxy
{
    public static class Proxy
    {
        public static event Action<Message> ProxyPacketReceived;

        private static TcpClient _client = new TcpClient();
        private static ProxyPacketSplitter _splitter = new ProxyPacketSplitter();

        private static int _retries = 2;

        private static void SendData(string data)
        {
            try
            {
                var stream = _client.GetStream();
                var utf8 = new UTF8Encoding();
                var bb = utf8.GetBytes(data);
                stream.Write(bb, 0, bb.Length);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                _retries = 2;
                //ConnectToProxy();
            }
        }

        private static void ReceiveData()
        {
            var buffer = new byte[2048];
            try
            {
                while (_client.Connected)
                {
                    var size = _client.GetStream().Read(buffer, 0, buffer.Length);
                    var data = Encoding.UTF8.GetString(buffer.Take(size).ToArray());
                    //Console.WriteLine($"[Proxy] raw output: {data}");
                    _splitter.Append(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void ProxyPacketAnalysisLoop()
        {
            while (_client.Connected)
            {
                var successDequeue = _splitter.Packets.TryDequeue(out var data);
                if (!successDequeue)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (data.Contains(":tcc"))
                {
                    HandleGpkData(data.Substring(data.IndexOf(":tcc", StringComparison.Ordinal)));
                }
                else
                {
                    var split = data.Split(new[] { "\t::\t" }, StringSplitOptions.None);

                    var type = split[0];
                    if (type == "output")
                    {
                        //Console.WriteLine($"[Proxy] received output: {split[3]}");
                        var channel = UInt32.Parse(split[1]);
                        var author = split[2];
                        var message = split[3];

                        HandleProxyOutput(author, channel, AddFontTagsIfMissing(message));

                        ////TODO: parse author
                        //var msg = data.StartsWith("<font", StringComparison.InvariantCultureIgnoreCase) ? data : "<FONT>" + data;
                        //msg = msg.EndsWith("</font>", StringComparison.InvariantCultureIgnoreCase) ? msg : msg + "</FONT>";
                        //PacketHandler.HandleCommandOutput(msg);
                    }
                    else if (type == "packet")
                    {
                        //Console.WriteLine($"[Proxy] received packet: {split[2]}");
                        var dir = Boolean.Parse(split[1])
                            ? MessageDirection.ServerToClient
                            : MessageDirection.ClientToServer;
                        ProxyPacketReceived?.Invoke(new Message(DateTime.UtcNow, dir, split[2].Substring(4)));
                    }
                    else if (type == "setval")
                    {
                        var propName = split[1];
                        var val = split[2];
                        SetProperty(propName, val);
                        //Console.WriteLine($"[Proxy] received setval: {split[1]} - {split[2]}");
                    }
                }


            }
            // ReSharper disable once FunctionNeverReturns
        }

        internal static void ReturnToLobby()
        {
            SendData("return_to_lobby");
        }

        public static void HandleProxyOutput(string author, uint channel, string message)
        {

            if (author == "undefined") author = "System";
            if (!ChatWindowManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
                ChatWindowManager.Instance.CachePrivateMessage(channel, author, message);
            else
                ChatWindowManager.Instance.AddChatMessage(
                    new ChatMessage(((ChatChannel)ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(x =>
                                         x.Id == channel && x.Joined).Index + 11), author, message));
        }

        public static void HandleGpkData(string data)
        {
            const string chatModeCmd = ":tcc-chatMode:";
            const string uiModeCmd = ":tcc-uiMode:";
            const string unkString = "Unknown command ";
            data = data.Replace(unkString, "").Replace("\"", "").Replace(".", "");
            if (data.StartsWith(chatModeCmd))
            {
                var chatMode = data.Replace(chatModeCmd, "");
                SessionManager.InGameChatOpen = chatMode == "1" || chatMode == "true"; //too lazy
            }
            else if (data.StartsWith(uiModeCmd))
            {
                var uiMode = data.Replace(uiModeCmd, "");
                SessionManager.InGameUiOn = uiMode == "1" || uiMode == "true"; //too lazy
            }
        }

        private static void SetProperty(string propName, string val)
        {
            var pi = typeof(Proxy).GetProperty(propName);
            pi?.SetValue(null, Convert.ChangeType(val, pi.PropertyType));
        }

        private static string AddFontTagsIfMissing(string msg)
        {
            var sb = new StringBuilder();
            if (!msg.StartsWith("<font", StringComparison.InvariantCultureIgnoreCase))
            {

                if (msg.IndexOf("<font", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    sb.Append("<font>");
                    sb.Append(msg.Substring(0, msg.IndexOf("<font", StringComparison.OrdinalIgnoreCase)));
                    sb.Append("</font>");
                    sb.Append(msg.Substring(msg.IndexOf("<font", StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    sb.Append("<font>");
                    sb.Append(msg);
                    sb.Append("</font>");
                }
            }
            else sb.Append(msg);
            var openCount = Regex.Matches(msg, "<font").Count;
            var closeCount = Regex.Matches(msg, "</font>").Count;
            if (openCount > closeCount) sb.Append("</font>");
            return sb.ToString();
        }
        public static bool IsConnected => _client.Connected;
        public static bool IsFpsUtilsAvailable { get; [UsedImplicitly] set; }

        public static void ConnectToProxy()
        {
            try
            {
                if (_client.Client != null && _client.Connected) return;
                ChatWindowManager.Instance.AddTccMessage("Trying to connect to tera-proxy...");

                _client = new TcpClient();
                _client.Connect("127.0.0.50", 9550);
                ChatWindowManager.Instance.AddTccMessage("Connected to tera-proxy.");
                WindowManager.FloatingButton.NotifyExtended("Proxy", "Successfully connected to tera-proxy.", NotificationType.Success);
                var t = new Thread(ReceiveData);
                var analysisThread = new Thread(ProxyPacketAnalysisLoop);
                t.Start();
                analysisThread.Start();
                InitStub();
            }
            catch (Exception e)
            {
                _retries--;
                Log.F("Failed to connect to proxy: "+ e.Message);
                Task.Delay(2000).ContinueWith(t =>
                {
                    if (_retries <= 0)
                    {
                        ChatWindowManager.Instance.AddTccMessage("Maximum retries exceeded. tera-proxy functionalities won't be available.");
                        WindowManager.FloatingButton.NotifyExtended("Proxy", "Unable to connect to tera-proxy. Advanced functionalities won't be available.", NotificationType.Error);
                        _retries = 2;
                        return;
                    }
                    ConnectToProxy();
                });
            }
        }
        public static void CloseConnection()
        {
            try
            {
                _client?.GetStream().Close();
                _client?.Client?.Close();
                _client?.Close();
            }
            catch
            {
                // ignored
            }
        }

        private static void InitStub()
        {
            var sb = new StringBuilder("init_stub");
            sb.Append("&use_lfg=");
            sb.Append(Settings.SettingsHolder.LfgEnabled.ToString().ToLower());

            SendData(sb.ToString());
        }
        public static void ChatLinkData(string linkData)
        {
            // linkData = T#####DATA
            if (linkData == null) return;
            var sb = new StringBuilder("chat_link");
            sb.Append("&");
            sb.Append(":tcc:");
            sb.Append(linkData.Replace("#####", ":tcc:"));
            sb.Append(":tcc:");
            File.AppendAllText("link-test.txt", sb + "\n");
            SendData(sb.ToString());
        }
        public static void LootSettings()
        {
            var sb = new StringBuilder("loot_settings");
            SendData(sb.ToString());
        }
        public static void RequestPartyInfo(uint id)
        {
            var sb = new StringBuilder("lfg_party_req");
            sb.Append("&id=");
            sb.Append(id);

            SendData(sb.ToString());
        }
        public static void ApplyToLfg(uint id)
        {
            var sb = new StringBuilder("lfg_apply_req");
            sb.Append("&id=");
            sb.Append(id);

            SendData(sb.ToString());
        }
        public static void FriendRequest(string name, string message)
        {
            var sb = new StringBuilder("friend_req");
            sb.Append("&name=");
            sb.Append(name);
            sb.Append("&msg=");
            sb.Append(message);

            SendData(sb.ToString());
        }
        public static void UnfriendUser(string name)
        {
            var sb = new StringBuilder("unfriend");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void BlockUser(string name)
        {
            var sb = new StringBuilder("block");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void SetInvitePower(uint serverId, uint playerId, bool v)
        {
            var sb = new StringBuilder("power_change");
            sb.Append("&sId=");
            sb.Append(serverId);
            sb.Append("&pId=");
            sb.Append(playerId);
            sb.Append("&power=");
            sb.Append(v ? 1 : 0);

            SendData(sb.ToString());
        }
        public static void PublicizeLfg()
        {
            var sb = new StringBuilder("lfg_publicize");
            SendData(sb.ToString());
        }
        public static void RemoveLfg()
        {
            var sb = new StringBuilder("lfg_remove");
            SendData(sb.ToString());
        }
        public static void UnblockUser(string name)
        {
            var sb = new StringBuilder("unblock");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void RequestLfgList(int min = 60, int max = 65)
        {
            var sb = new StringBuilder("lfg_request_list");
            sb.Append("&minlvl=");
            sb.Append(min);
            sb.Append("&maxlvl=");
            sb.Append(max);
            SendData(sb.ToString());
        }
        public static void AskInteractive(uint srvId, string name)
        {
            var sb = new StringBuilder("ask_int");
            sb.Append("&srvId=");
            sb.Append(srvId);
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void DelegateLeader(uint serverId, uint playerId)
        {
            var sb = new StringBuilder("leader");
            sb.Append("&sId=");
            sb.Append(serverId);
            sb.Append("&pId=");
            sb.Append(playerId);

            SendData(sb.ToString());
        }
        public static void KickMember(uint serverId, uint playerId)
        {
            var sb = new StringBuilder("kick");
            sb.Append("&sId=");
            sb.Append(serverId);
            sb.Append("&pId=");
            sb.Append(playerId);

            SendData(sb.ToString());
        }
        public static void Inspect(string name)
        {
            var sb = new StringBuilder("inspect");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void PartyInvite(string name)
        {
            var sb = new StringBuilder("inv_party");
            sb.Append("&name=");
            sb.Append(name);
            sb.Append("&raid=");
            sb.Append(WindowManager.GroupWindow.VM.Raid ? 1 : 0);

            SendData(sb.ToString());
        }
        public static void GuildInvite(string name)
        {
            var sb = new StringBuilder("inv_guild");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void DeclineBrokerOffer(uint playerId, uint listingId)
        {
            var sb = new StringBuilder("tb_decline");
            sb.Append("&player=");
            sb.Append(playerId);
            sb.Append("&listing=");
            sb.Append(listingId);

            SendData(sb.ToString());

        }
        public static void AcceptBrokerOffer(uint playerId, uint listingId)
        {
            var sb = new StringBuilder("tb_accept");
            sb.Append("&player=");
            sb.Append(playerId);
            sb.Append("&listing=");
            sb.Append(listingId);

            SendData(sb.ToString());

        }
        public static void DeclineApply(uint playerId)
        {
            var sb = new StringBuilder("apply_decline");
            sb.Append("&player=");
            sb.Append(playerId);

            SendData(sb.ToString());
        }
        public static void ExTooltip(long itemUid, string ownerName)
        {
            var sb = new StringBuilder("ex_tooltip");
            sb.Append("&uid=");
            sb.Append(itemUid);
            sb.Append("&name=");
            sb.Append(ownerName);

            SendData(sb.ToString());
        }
        public static void NondbItemInfo(uint itemId)
        {
            var sb = new StringBuilder("nondb_info");
            sb.Append("&id=");
            sb.Append(itemId);

            SendData(sb.ToString());
        }
        public static void RequestNextLfgPage(int page)
        {
            var sb = new StringBuilder("lfg_page_req");
            sb.Append("&page=");
            sb.Append(page);
            SendData(sb.ToString());
        }
        public static void RegisterLfg(string msg, bool raid)
        {
            var sb = new StringBuilder("lfg_register");
            sb.Append("&msg=");
            sb.Append(msg);
            sb.Append("&raid=");
            sb.Append(raid.ToString().ToLower());
            SendData(sb.ToString());
        }
        public static void DisbandParty()
        {
            var sb = new StringBuilder("disband_group");
            SendData(sb.ToString());
        }
        public static void ResetInstance()
        {
            var sb = new StringBuilder("reset_instance");
            SendData(sb.ToString());
        }
        public static void LeaveParty()
        {
            var sb = new StringBuilder("leave_party");
            SendData(sb.ToString());
        }
        public static void RequestCandidates()
        {
            var sb = new StringBuilder("request_candidates");
            SendData(sb.ToString());
        }
        public static void ForceSystemMessage(string msg, string opcode)
        {
            var opc = PacketAnalyzer.Factory.SystemMessageNamer.GetCode(opcode);
            var badOpc = msg.Split('\v')[0];
            if (badOpc == "@0") msg = msg.Replace(badOpc, "@" + opc);
            var sb = new StringBuilder("force_sysmsg");
            sb.Append("&msg=");
            sb.Append(msg);

            SendData(sb.ToString());

        }
        public static void SendCommand(string command)
        {
            var sb = new StringBuilder("command");
            sb.Append("&cmd=");
            sb.Append(command);
            SendData(sb.ToString());
        }
    }
}
