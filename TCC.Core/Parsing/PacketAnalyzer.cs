using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FoglioUtils;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Sniffing;
using TCC.TeraCommon.Sniffing;
using TCC.Utilities;
using TCC.ViewModels;
using TCC.Windows;
using TeraPacketParser;
using TeraPacketParser.Messages;
using Server = TCC.TeraCommon.Game.Server;

namespace TCC.Parsing
{
    public static class PacketAnalyzer
    {
        public static event Action ProcessorReady;
        public static ITeraSniffer Sniffer;
        public static MessageFactory Factory;
        public static MessageProcessor Processor;
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();
        public static Thread AnalysisThread;
        public static int AnalysisThreadId;
        public static NewProcessor NewProcessor { get; set; }

        private static void Init()
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

            Game.Server = new Server("", "", "", 0);
            ProcessorReady += InstallHooks;

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
        private static void InstallHooks()
        {
            NewProcessor.Hook<C_CHECK_VERSION>(PacketHandler.HandleCheckVersion);
            NewProcessor.Hook<C_LOGIN_ARBITER>(PacketHandler.HandleLoginArbiter);
            NewProcessor.Hook<S_GET_USER_LIST>(PacketHandler.HandleGetUserList);
            NewProcessor.Hook<S_LOGIN>(PacketHandler.HandleLogin);
        }

        public static async void InitAsync()
        {
            await Task.Factory.StartNew(Init);

            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
        }
        private static void PacketAnalysisLoop()
        {
            AnalysisThreadId = MiscUtils.GetCurrentThreadId();
            NewProcessor = new NewProcessor();
            App.BaseDispatcher.BeginInvoke(new Action(() => { ProcessorReady?.Invoke(); }));

            while (true)
            {
                if (!Packets.TryDequeue(out var pkt))
                {
                    Thread.Sleep(1);
                    continue;
                }

                var msg = Factory.Create(pkt);
                NewProcessor.Handle(msg);
                //Processor.Process(msg);
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private static void OnNewConnection(Server srv)
        {
            Game.Server = srv;
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            ChatWindowManager.Instance.AddTccMessage($"Connected to {srv.Name}.");
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

            ChatWindowManager.Instance.AddTccMessage("Disconnected from the server.");
            WindowManager.FloatingButton.NotifyExtended("TCC", "Disconnected", NotificationType.Normal);

            WindowManager.ViewModels.Group.ClearAllAbnormalities();
            WindowManager.ViewModels.Dashboard.UpdateBuffs();
            WindowManager.ViewModels.Dashboard.SaveCharacters();
            Game.Me.ClearAbnormalities();
            EntityManager.ClearNPC();
            WindowManager.ViewModels.Cooldowns.ClearSkills(); // TODO: hook connection to these too
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            ProxyInterface.Instance.Disconnect();
            Game.Logged = false;
            Game.LoadingScreen = true;

            if (App.ToolboxMode && UpdateManager.UpdateAvailable) App.Close();
        }

        public static void EnqueuePacket(Message message)
        {
            Packets.Enqueue(message);
        }

    }
    public class NewProcessor
    {
        private ConcurrentDictionary<Type, List<Delegate>> _hooks { get; }

        private readonly object _lock = new object();

        public NewProcessor()
        {
            _hooks = new ConcurrentDictionary<Type, List<Delegate>>();
        }

        public void Hook<T>(Action<T> action)
        {
            lock (_lock)
            {
                if (!_hooks.TryGetValue(typeof(T), out _)) _hooks[typeof(T)] = new List<Delegate>();
                if (!_hooks[typeof(T)].Contains(action)) _hooks[typeof(T)].Add(action);
            }
        }
        public void Unhook<T>(Action<T> action)
        {
            lock (_lock)
            {
                if (!_hooks.TryGetValue(typeof(T), out var handlers)) return;
                handlers.Remove(action);
            }
        }
        public void Handle(ParsedMessage msg)
        {
            if (!_hooks.TryGetValue(msg.GetType(), out var handlers) || handlers == null) return;
            lock (_lock)
            {
                //Log.All($"Handling {msg.GetType().Name}");
                handlers.ForEach(del => del.DynamicInvoke(msg));
            }
        }
    }

}