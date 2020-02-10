using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TCC.Data;
using TCC.Exceptions;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Loader;
using TCC.Sniffing;
using TCC.TeraCommon.Sniffing;
using TCC.Update;
using TCC.Utils;
using TCC.Windows;
using TeraPacketParser;
using TeraPacketParser.Messages;
using MessageBoxImage = TCC.Data.MessageBoxImage;
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
            Factory = new MessageFactory();

            Sniffer = SnifferFactory.Create();
            Sniffer.NewConnection += OnNewConnection;
            Sniffer.EndConnection += OnEndConnection;
            Sniffer.MessageReceived += EnqueuePacket;
            Sniffer.Enabled = true;

            AnalysisThread = new Thread(PacketAnalysisLoop) { Name = "Analysis" };
            AnalysisThread.Start();
        }

        public static async Task InitAsync()
        {
            ProcessorReady += () => App.BaseDispatcher.Invoke(() =>
            {
                try
                {
                    ModuleLoader.LoadModules(App.BasePath);
                }
                catch (FileLoadException fle)
                {
                    TccMessageBox.Show("TCC module loader",
                        $"An error occured while loading {fle.FileName}. TCC will now close. You can find more info about this error in TERA Dps discord #known-issues channel.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Close();
                }
                catch (FileNotFoundException fnfe)
                {
                    TccMessageBox.Show("TCC module loader",
                        $"An error occured while loading {Path.GetFileName(fnfe.FileName)}. TCC will now close. You can find more info about this error in TERA Dps discord #known-issues channel.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Close();
                }
            });

            await Task.Factory.StartNew(Init);
            Log.N("TCC", "Ready to connect.", NotificationType.Normal);
        }
        private static void PacketAnalysisLoop()
        {
            Processor = new MessageProcessor();
            Processor.Hook<C_CHECK_VERSION>(OnCheckVersion);
            Processor.Hook<C_LOGIN_ARBITER>(OnLoginArbiter);

            if (ProcessorReady != null) App.BaseDispatcher.InvokeAsync(ProcessorReady);
            while (true)
            {
                if (!Packets.TryDequeue(out var pkt))
                {
                    Thread.Sleep(1);
                    continue;
                }

                ParsedMessage msg;
                try
                {
                    msg = Factory.Create(pkt);
                }
                catch (Exception ex)
                {
                    var opcName = Factory.OpCodeNamer.GetName(pkt.OpCode);
                    throw new PacketParseException($"Failed to parse packet {opcName}", ex, opcName, pkt.Data.Array);
                }
                Processor.Handle(msg);
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private static void OnNewConnection(Server srv)
        {
            Game.Server = srv;
            WindowManager.TrayIcon.Connected = true;
            WindowManager.TrayIcon.Text = $"{App.AppVersion} - connected";
            Log.N("TCC", $"Connected to {srv.Name}", NotificationType.Success);

            _ = ProxyInterface.Instance.Init();

            if (!App.Settings.DontShowFUBH) App.FUBH();

            //if (Game.Server.Region == "EU")
            //    TccMessageBox.Show("WARNING",
            //        "Official statement from Gameforge:\n\n don't combine partners or pets! It will lock you out of your character permanently.\n\n This message will keep showing until next release.");
        }
        private static void OnEndConnection()
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);

            Log.N("TCC", "Disconnected", NotificationType.Normal);
            WindowManager.TrayIcon.Connected = false;
            WindowManager.TrayIcon.Text = $"{App.AppVersion} - not connected";

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
            if (!File.Exists(opcPath))
            {
                if (Sniffer is ToolboxSniffer tbs)
                {
                    if (!Directory.Exists(Path.Combine(App.DataPath, "opcodes")))
                        Directory.CreateDirectory(Path.Combine(App.DataPath, "opcodes"));
                    if (!await tbs.ControlConnection.DumpMap(opcPath, "protocol"))
                    {
                        TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                        App.Close();
                        return;
                    }
                }
                else
                {
                    if (OpcodeDownloader.DownloadOpcodesIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/"))) return;
                    TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                    App.Close();
                    return;
                }
            }
            var opcNamer = new OpCodeNamer(opcPath);
            Factory.Set(p.Versions[0], opcNamer);
            Sniffer.Connected = true;
        }
        private static async void OnLoginArbiter(C_LOGIN_ARBITER p)
        {
            var path = File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.ReleaseVersion / 100}.map"))
                       ?
                       Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.ReleaseVersion / 100}.map")
                       :
                       File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map"))
                           ? Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map")
                           : "";

            if (path == "")
            {
                var destPath = Path.Combine(App.DataPath, $"opcodes/sysmsg.{Factory.Version}.map").Replace("\\", "/");
                if (Sniffer.Connected && Sniffer is ToolboxSniffer tbs)
                {
                    if (await tbs.ControlConnection.DumpMap(destPath, "sysmsg"))
                    {
                        Factory.SystemMessageNamer = new OpCodeNamer(destPath);
                        return;
                    }
                }
                else
                {
                    if (OpcodeDownloader.DownloadSysmsgIfNotExist(Factory.Version, Path.Combine(App.DataPath, "opcodes/"), Factory.ReleaseVersion))
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

            Log.N("TCC", $"Release Version: {Factory.ReleaseVersion / 100D}", NotificationType.Normal); //by HQ 20190209
        }
    }
}