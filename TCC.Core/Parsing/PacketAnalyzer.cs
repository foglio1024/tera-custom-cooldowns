using FoglioUtils;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Sniffing;
using TCC.TeraCommon.Sniffing;
using TCC.Windows;
using TeraPacketParser;
using TeraPacketParser.Messages;
using Server = TCC.TeraCommon.Game.Server;

namespace TCC.Parsing
{
    public static class PacketAnalyzer
    {
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();

        public static event Action ProcessorReady;

        public static ITeraSniffer Sniffer { get; private set; }
        public static MessageFactory Factory { get; private set; }
        public static MessageProcessor Processor { get; private set; }

        public static Thread AnalysisThread { get; private set; }

        private static void Init()
        {
            Sniffer = SnifferFactory.Create();
            Factory = new MessageFactory();

            Sniffer.NewConnection += OnNewConnection;
            Sniffer.EndConnection += OnEndConnection;
            Sniffer.MessageReceived += EnqueuePacket;
            Sniffer.Enabled = true;

            AnalysisThread = new Thread(PacketAnalysisLoop) { Name = "Analysis" };
            AnalysisThread.Start();
        }

        public static async void InitAsync()
        {
            await Task.Factory.StartNew(Init);
            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
        }
        private static void PacketAnalysisLoop()
        {
            Processor = new MessageProcessor();
            Processor.Hook<C_CHECK_VERSION>(OnCheckVersion);
            Processor.Hook<C_LOGIN_ARBITER>(OnLoginArbiter);

            if (ProcessorReady != null) App.BaseDispatcher.BeginInvoke(ProcessorReady);
            while (true)
            {
                if (!Packets.TryDequeue(out var pkt))
                {
                    Thread.Sleep(1);
                    continue;
                }
                Processor.Handle(Factory.Create(pkt));
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private static void OnNewConnection(Server srv)
        {
            Game.Server = srv;
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            WindowManager.FloatingButton.NotifyExtended("TCC", $"Connected to {srv.Name}", NotificationType.Success);

            if (!App.Settings.DontShowFUBH) App.FUBH();

            //if (Game.Server.Region == "EU")
            //    TccMessageBox.Show("WARNING",
            //        "Official statement from Gameforge:\n\n don't combine partners or pets! It will lock you out of your character permanently.\n\n This message will keep showing until next release.");
        }
        private static void OnEndConnection()
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);

            WindowManager.FloatingButton.NotifyExtended("TCC", "Disconnected", NotificationType.Normal);
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;

            ProxyInterface.Instance.Disconnect();

            if (App.ToolboxMode && UpdateManager.UpdateAvailable) App.Close();
        }

        public static void EnqueuePacket(Message message)
        {
            Packets.Enqueue(message);
        }

        private static async void OnCheckVersion(C_CHECK_VERSION p)
        {
            var opcPath = Path.Combine(App.DataPath, $"opcodes/protocol.{p.Versions[0]}.map").Replace("\\", "/");
            OpcodeDownloader.DownloadOpcodesIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/"));
            if (!File.Exists(opcPath))
            {
                if (Sniffer is ToolboxSniffer tbs)
                {
                    if (!await tbs.ControlConnection.DumpMap(opcPath, "protocol"))
                    {
                        TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                        App.Close();
                        return;
                    }
                }
                else
                {
                    TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                    App.Close();
                    return;
                }
            }
            var opcNamer = new OpCodeNamer(opcPath);
            Factory = new MessageFactory(p.Versions[0], opcNamer);
            Sniffer.Connected = true;
        }
        private static async void OnLoginArbiter(C_LOGIN_ARBITER p)
        {
            OpcodeDownloader.DownloadSysmsgIfNotExist(Factory.Version, Path.Combine(App.DataPath, "opcodes/"), Factory.ReleaseVersion);
            var path = File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.ReleaseVersion / 100}.map"))
                       ?
                       Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.ReleaseVersion / 100}.map")
                       :
                       File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map"))
                           ? Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map")
                           : "";

            if (path == "")
            {
                if (Sniffer.Connected && Sniffer is ToolboxSniffer tbs)
                {
                    var destPath = Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map").Replace("\\", "/");
                    if (await tbs.ControlConnection.DumpMap(destPath, "sysmsg"))
                    {
                        Factory.SystemMessageNamer = new OpCodeNamer(destPath);
                        return;
                    }
                }
                TccMessageBox.Show($"sysmsg.{Factory.ReleaseVersion / 100}.map or sysmsg.{Factory.Version}.map not found.\nWait for update or use tcc-stub to automatically retreive sysmsg files from game client.\nTCC will now close.", MessageBoxType.Error);
                App.Close();
                return;
            }
            Factory.ReloadSysMsg(path);

            WindowManager.FloatingButton.NotifyExtended("TCC", $"Release Version: {Factory.ReleaseVersion / 100D}", NotificationType.Normal); //by HQ 20190209
        }
    }
}