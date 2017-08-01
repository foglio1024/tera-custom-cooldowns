using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC
{
    public static class ProxyInterop
    {
        static TcpClient client = new TcpClient();
        static int retries = 2;
        public static bool IsConnected
        {
            get
            {
                return client.Connected;
            }
        }

        static void ReceiveData()
        {
            var buffer = new byte[2048];
            try
            {
                while (client.Connected)
                {
                    var size = client.GetStream().Read(buffer, 0, buffer.Length);
                    var msg = "<FONT>"+Encoding.UTF8.GetString(buffer.Take(size).ToArray())+"</FONT>";
                    PacketProcessor.HandleCommandOutput(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ConnectToProxy()
        {
            try
            {
                if (client.Client != null && client.Connected) return;
                Debug.WriteLine("Connecting...");
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System", "<FONT>Trying to connect to tera-proxy...</FONT>"));

                client = new TcpClient();
                client.Connect("127.0.0.50", 9550);
                Debug.WriteLine("Connected");
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System", "<FONT>Connected to tera-proxy.</FONT>"));
                var t = new Thread(new ThreadStart(ReceiveData));
                t.Start();
            }
            catch (Exception e)
            {
                retries--;
                Debug.WriteLine(e.Message);
                Task.Delay(2000).ContinueWith(t =>
                {
                    if (retries <= 0)
                    {
                        Debug.WriteLine("Maximum retries exceeded...");
                        ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System", "<FONT>Maximum retries exceeded. tera-proxy functionalities won't be available.</FONT>"));

                        retries = 2;
                        return;
                    }
                    ConnectToProxy();
                });

            }
        }

        internal static void SendLinkData(string linkData)
        {
            // linkData = T#####DATA
            if (linkData == null) return;
            var sb = new StringBuilder("chat_link");
            sb.Append("&");
            sb.Append(":tcc:");
            sb.Append(linkData.Replace("#####", ":tcc:"));
            sb.Append(":tcc:");
            System.IO.File.AppendAllText("link-test.txt", sb.ToString() + "\n");
            SendData(sb.ToString());
        }

        static void SendData(string data)
        {
            try
            {
                var stream = client.GetStream();
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] bb = utf8.GetBytes(data);
                stream.Write(bb, 0, bb.Length);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                retries = 2;
                //ConnectToProxy();
            }
        }

        internal static void SendRequestPartyInfo(int id)
        {
            var sb = new StringBuilder("lfg_party_req");
            sb.Append("&id=");
            sb.Append(id);

            SendData(sb.ToString());
        }

        public static void SendExTooltipMessage(long itemUid, string ownerName)
        {
            var sb = new StringBuilder("ex_tooltip");
            sb.Append("&uid=");
            sb.Append(itemUid);
            sb.Append("&name=");
            sb.Append(ownerName);

            SendData(sb.ToString());
        }
        public static void SendNondbItemInfoMessage(uint itemId)
        {
            var sb = new StringBuilder("nondb_info");
            sb.Append("&id=");
            sb.Append(itemId);

            SendData(sb.ToString());
        }

        public static void SendFriendRequestMessage(string name, string message)
        {
            var sb = new StringBuilder("friend_req");
            sb.Append("&name=");
            sb.Append(name);
            sb.Append("&msg=");
            sb.Append(message);

            SendData(sb.ToString());
        }
        public static void SendUnfriendUserMessage(string name)
        {
            var sb = new StringBuilder("unfriend");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }

        public static void SendBlockUserMessage(string name)
        {
            var sb = new StringBuilder("block");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }

        internal static void SendGrantRevokeInvite(uint serverId, uint playerId, bool v)
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

        public static void SendUnblockUserMessage(string name)
        {
            var sb = new StringBuilder("unblock");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void SendAskInteractiveMessage(uint srvId, string name)
        {
            var sb = new StringBuilder("ask_int");
            sb.Append("&srvId=");
            sb.Append(srvId);
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }

        internal static void SendDelegateLeader(uint serverId, uint playerId)
        {
            var sb = new StringBuilder("leader");
            sb.Append("&sId=");
            sb.Append(serverId);
            sb.Append("&pId=");
            sb.Append(playerId);

            SendData(sb.ToString());
        }

        public static void SendInspect(string name)
        {
            var sb = new StringBuilder("inspect");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void SendPartyInvite(string name)
        {
            var sb = new StringBuilder("inv_party");
            sb.Append("&name=");
            sb.Append(name);
            sb.Append("&raid=");
            sb.Append(GroupWindowViewModel.Instance.Raid ? 1 : 0);

            SendData(sb.ToString());
        }
        public static void SendGuildInvite(string name)
        {
            var sb = new StringBuilder("inv_guild");
            sb.Append("&name=");
            sb.Append(name);

            SendData(sb.ToString());
        }
        public static void SendTradeBrokerDecline(uint playerId, uint listingId)
        {
            var sb = new StringBuilder("tb_decline");
            sb.Append("&player=");
            sb.Append(playerId);
            sb.Append("&listing=");
            sb.Append(listingId);

            SendData(sb.ToString());

        }
        public static void SendTradeBrokerAccept(uint playerId, uint listingId)
        {
            var sb = new StringBuilder("tb_accept");
            sb.Append("&player=");
            sb.Append(playerId);
            sb.Append("&listing=");
            sb.Append(listingId);

            SendData(sb.ToString());

        }
        public static void SendDeclineApply(uint playerId)
        {
            var sb = new StringBuilder("apply_decline");
            sb.Append("&player=");
            sb.Append(playerId);

            SendData(sb.ToString());
        }
        public static void CloseConnection()
        {
            try
            {
                client?.GetStream().Close();
                client?.Client?.Close();
                client?.Close();

            }
            catch (Exception)
            {


            }
        }
    }
}
