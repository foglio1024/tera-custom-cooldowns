using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Settings;
using TCC.Sniffing;
using TCC.TeraCommon.Game;
using TCC.TeraCommon.Sniffing;
using TCC.ViewModels;
using TeraPacketParser;
using Server = TCC.TeraCommon.Game.Server;

namespace TCC.Parsing
{
    public static class PacketAnalyzer
    {
        public static ITeraSniffer Sniffer;
        public static MessageFactory Factory;
        public static MessageProcessor Processor;
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();
        public static Thread AnalysisThread;
        public static int AnalysisThreadId;
        public static void Init()
        {
            switch (App.Settings.CaptureMode)
            {
                case CaptureMode.Npcap when !App.ToolboxMode:
                case CaptureMode.RawSockets when !App.ToolboxMode:
                    Sniffer = new TeraSniffer();
                    break;
                default:
                    Sniffer = new ToolboxSniffer();
                    break;
            }
            Sniffer.NewConnection += OnNewConnection;
            Sniffer.EndConnection += OnEndConnection;
            Sniffer.MessageReceived += EnqueuePacket;

            SessionManager.Server = new Server("", "", "", 0);

            Factory = new MessageFactory();
            Processor = new MessageProcessor();
            if (AnalysisThread == null)
            {
                Log.All("Analysis thread not running, starting it...");
                AnalysisThread = new Thread(PacketAnalysisLoop) { Name = "Analysis" };
                AnalysisThread.Start();
            }
            else
            {
                Log.All("Analysis already running, skipping...");
            }

            Sniffer.Enabled = true;
        }

        public static async void InitAsync()
        {
            await Task.Factory.StartNew(Init);
            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
        }
        private static void PacketAnalysisLoop()
        {
            AnalysisThreadId = FoglioUtils.MiscUtils.GetCurrentThreadId();
            while (true)
            {
                if (!Packets.TryDequeue(out var msg))
                {
                    Thread.Sleep(1);
                    continue;
                }
                Processor.Process(Factory.Create(msg));
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private static void OnNewConnection(Server srv)
        {
            SessionManager.Server = srv;
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            ChatWindowManager.Instance.AddTccMessage($"Connected to {srv.Name}.");
            WindowManager.FloatingButton.NotifyExtended("TCC", $"Connected to {srv.Name}", NotificationType.Success);

            if (!App.Settings.DontShowFUBH) App.FUBH();
        }
        private static void OnEndConnection()
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);

            ChatWindowManager.Instance.AddTccMessage("Disconnected from the server.");
            WindowManager.FloatingButton.NotifyExtended("TCC", "Disconnected", NotificationType.Warning);

            WindowManager.GroupWindow.VM.ClearAllAbnormalities();
            WindowManager.Dashboard.VM.UpdateBuffs();
            WindowManager.Dashboard.VM.SaveCharacters();
            SessionManager.CurrentPlayer.ClearAbnormalities();
            EntityManager.ClearNPC();
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            ProxyInterface.Instance.Disconnect(); //ProxyOld.CloseConnection();
            SessionManager.Logged = false;
            SessionManager.LoadingScreen = true;
        }

        public static void EnqueuePacket(Message message)
        {
            Packets.Enqueue(message);
        }
    }
}