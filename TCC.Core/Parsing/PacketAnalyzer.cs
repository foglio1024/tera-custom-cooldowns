using System.Collections.Concurrent;
using System.Threading;
using TCC.Sniffing;
using TCC.TeraCommon;

namespace TCC.Parsing
{
    public static class PacketAnalyzer
    {
        public static MessageFactory Factory;
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();
        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += Packets.Enqueue;
            Proxy.Proxy.ProxyPacketReceived += Packets.Enqueue;
            Factory = new MessageFactory();

            var analysisThread = new Thread(PacketAnalysisLoop);
            analysisThread.Start();
        }
        private static void PacketAnalysisLoop()
        {
            while (true)
            {
                if (!Packets.TryDequeue(out var msg))
                {
                    Thread.Sleep(1);
                    continue;
                }
                Factory.Process(Factory.Create(msg));
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}