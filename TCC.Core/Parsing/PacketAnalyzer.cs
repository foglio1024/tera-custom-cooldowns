using System.Collections.Concurrent;
using System.Threading;
using TCC.Data;
using TCC.Sniffing;
using TCC.TeraCommon;
using TCC.TeraCommon.Game;
using TCC.ViewModels;

namespace TCC.Parsing
{
    public static class PacketAnalyzer
    {
        public static MessageFactory Factory;
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();
        public static Thread AnalysisThread;
        public static int AnalysisThreadId;
        public static void Init()
        {
            TeraSniffer.Instance.NewConnection += OnNewConnection;
            TeraSniffer.Instance.EndConnection += OnEndConnection;

            TeraSniffer.Instance.MessageReceived += Packets.Enqueue;
            Proxy.Proxy.ProxyPacketReceived += Packets.Enqueue;

            Factory = new MessageFactory();
            if (AnalysisThread == null)
            {
                Log.All("Analysis thread not running, starting it...");
                AnalysisThread = new Thread(PacketAnalysisLoop) { Name = "Anal" };
                AnalysisThread.Start();
            }
            else
            {
                Log.All("Analysis already running, skipping...");
            }
            TeraSniffer.Instance.Enabled = true;
        }
        private static void PacketAnalysisLoop()
        {
            AnalysisThreadId = App.GetCurrentThreadId();
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
        private static void OnNewConnection(Server srv)
        {
            SessionManager.Server = srv;
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            ChatWindowManager.Instance.AddTccMessage($"Connected to {srv.Name}.");
            WindowManager.FloatingButton.NotifyExtended("TCC", $"Connected to {srv.Name}", NotificationType.Success);
        }
        private static void OnEndConnection()
        {
            ChatWindowManager.Instance.AddTccMessage("Disconnected from the server.");
            WindowManager.FloatingButton.NotifyExtended("TCC", "Disconnected", NotificationType.Warning);

            WindowManager.GroupWindow.VM.ClearAllAbnormalities();
            WindowManager.Dashboard.VM.UpdateBuffs();
            WindowManager.Dashboard.VM.SaveCharacters();
            SessionManager.CurrentPlayer.ClearAbnormalities();
            EntityManager.ClearNPC();
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            Proxy.Proxy.CloseConnection();
            SessionManager.Logged = false;
            SessionManager.LoadingScreen = true;
        }
    }
}