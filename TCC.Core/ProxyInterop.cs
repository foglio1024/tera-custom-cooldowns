using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
                Debug.WriteLine("Connecting...");
                if (client.Client == null) client = new TcpClient();
                if (client.Connected) return;
                client.Connect("127.0.0.50", 9550);
                Debug.WriteLine("Connected");
            }
            catch (Exception e)
            {
                retries--;
                Debug.WriteLine(e.Message);
                Task.Delay(2000).ContinueWith(t =>
                {
                    if (retries == 0)
                    {
                        Debug.WriteLine("Maximum retries exceeded...");
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
            }
        }

        public static void SendExTooltipMessage(ulong itemUid, string ownerName)
        {
            var sb = new StringBuilder("ex_tooltip");
            sb.Append("&uid=");
            sb.Append(itemUid);
            sb.Append("&name=");
            sb.Append(ownerName);

            SendData(sb.ToString());
        }
        public static void SendNondbItemInfoMessage(int itemId)
        {
            var sb = new StringBuilder("nondb_info");
            sb.Append("&id=");
            sb.Append(itemId);

            SendData(sb.ToString());
        }

        internal static void CloseConnection()
        {
            client?.Client.Close();
            client?.Close();
        }
    }
}
