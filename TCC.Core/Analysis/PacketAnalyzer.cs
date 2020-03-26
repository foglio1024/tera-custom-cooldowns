using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Exceptions;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Processing;
using TCC.Sniffing;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utils;
using TeraPacketParser;
using TeraPacketParser.Messages;
using TeraPacketParser.TeraCommon.Game;
using TeraPacketParser.TeraCommon.Sniffing;

namespace TCC.Analysis
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

            await Task.Factory.StartNew(Init);
            Log.N("TCC", SR.ReadyToConnect, NotificationType.None);
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
            if (Sniffer is TeraSniffer) DetectClientVersionFromFile();

            Game.Server = srv;
            WindowManager.TrayIcon.Connected = true;
            WindowManager.TrayIcon.Text = $"{App.AppVersion} - connected";

            _ = StubInterface.Instance.Init();

            if (!App.Settings.DontShowFUBH) App.FUBH();

            //if (Game.Server.Region == "EU")
            //    TccMessageBox.Show("WARNING",
            //        "Official statement from Gameforge:\n\n don't combine partners or pets! It will lock you out of your character permanently.\n\n This message will keep showing until next release.");
        }
        private static void OnEndConnection()
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);

            Log.N("TCC", SR.Disconnected, NotificationType.None);
            WindowManager.TrayIcon.Connected = false;
            WindowManager.TrayIcon.Text = $"{App.AppVersion} - not connected";

            StubInterface.Instance.Disconnect();

            if (App.ToolboxMode && UpdateManager.UpdateAvailable) App.Close();
        }

        public static void EnqueuePacket(Message message)
        {
            Packets.Enqueue(message);
        }

        private static void DetectClientVersionFromFile()
        {
            try
            {
                var process = Process.GetProcessesByName("TERA")[0];
                var fullPath = process?.MainModule?.FileName.Replace("TERA.exe", "ReleaseRevision.txt");
                if (fullPath == null) throw new FileNotFoundException("TERA.exe not found.");

                var txt = File.ReadAllLines(fullPath);
                foreach (var line in txt)
                {
                    if (!line.StartsWith("Version:")) continue;
                    var idx = line.IndexOf("Live-", StringComparison.InvariantCultureIgnoreCase);
                    var v = line.Substring(idx + 5);
                    var idx2 = v.IndexOf(' ');
                    v = v.Substring(0, idx2);
                    v = v.Replace(".", "");
                    Factory.ReleaseVersion = int.Parse(v);
                }
            }
            catch (Exception e)
            {
                throw new ClientVersionDetectionException("Failed to detect client version from file", e);
            }
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
                        TccMessageBox.Show(SR.UnknownClientVersion(p.Versions[0]), MessageBoxType.Error);
                        App.Close();
                        return;
                    }
                }
                else
                {
                    if (OpcodeDownloader.DownloadOpcodesIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/"))) return;
                    TccMessageBox.Show(SR.UnknownClientVersion(p.Versions[0]), MessageBoxType.Error);
                    App.Close();
                    return;
                }
            }

            OpCodeNamer opcNamer;
            try
            {
                opcNamer = new OpCodeNamer(opcPath);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case OverflowException _:
                    case ArgumentException _:
                        TccMessageBox.Show(SR.InvalidOpcodeFile(ex.Message), MessageBoxType.Error);
                        Log.F(ex.ToString());
                        App.Close();
                        break;
                }
                return;
            }

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

                TccMessageBox.Show(SR.InvalidSysMsgFile(Factory.ReleaseVersion / 100, Factory.Version), MessageBoxType.Error);
                App.Close();
                return;
            }
            Factory.ReloadSysMsg(path);
            
        }
    }
}