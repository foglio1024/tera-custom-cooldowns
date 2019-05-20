using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Settings;
using TCC.Sniffing;
using TCC.TeraCommon;
using TCC.ViewModels;
using Server = TCC.TeraCommon.Game.Server;

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

            TeraSniffer.Instance.MessageReceived += EnqueuePacket;
            //ProxyOld.ProxyPacketReceived += EnqueuePacket;

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

        public static async void InitAsync()
        {
            await Task.Factory.StartNew(Init);
            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
        }
        private static void PacketAnalysisLoop()
        {
            AnalysisThreadId = Utils.GetCurrentThreadId();
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

            App.FUBH();
        }
        private static void OnEndConnection()
        {
            Firebase.RegisterWebhook(SettingsHolder.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(SettingsHolder.WebhookUrlFieldBoss, false);

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