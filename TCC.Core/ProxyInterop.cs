using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TCC.ViewModels;

namespace TCC
{
    public static class ProxyInterop
    {
        static TcpClient client = new TcpClient();
        static int retries = 2;
        public static void ConnectToProxy()
        {
            try
            {
                if (client.Client != null && client.Connected) return;
                Debug.WriteLine("Connecting...");
                client = new TcpClient();
                client.Connect("127.0.0.50", 9550);
                Debug.WriteLine("Connected");
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
                        retries = 2;
                        return;
                    }
                    ConnectToProxy();
                });

            }
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
                ConnectToProxy();
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
            sb.Append(GroupWindowManager.Instance.Raid ? 1 : 0);

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

        public static void CloseConnection()
        {
            client?.Client?.Close();
            client?.Close();
        }
    }
}
