using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TCC.Utils;
using TCC.Utils.Exceptions;
using TeraDataLite;
using TeraPacketParser.Processing;
using TeraPacketParser.Sniffing;
using TeraPacketParser.TeraCommon.Game;
using TeraPacketParser.TeraCommon.Game.Services;
using TeraPacketParser.TeraCommon.Sniffing;

namespace TeraPacketParser.Analysis;

public static class PacketAnalyzer
{
    static readonly ConcurrentQueue<Message> Packets = new();

    public static event Action? ProcessorReady;

    public static ITeraSniffer Sniffer { get; private set; } = null!;
    public static MessageFactory? Factory { get; private set; }
    public static MessageProcessor Processor { get; private set; } = null!;
    public static Thread AnalysisThread { get; private set; } = null!;
    public static ServerDatabase ServerDatabase { get; private set; } = null!;

    public static async Task InitAsync(CaptureMode settingsCaptureMode, bool toolboxMode)
    {
        await Task.Factory.StartNew(() => Init(settingsCaptureMode, toolboxMode));
        Log.N("TCC", SR.ReadyToConnect, NotificationType.None);
    }

    public static void EnqueuePacket(Message message)
    {
        Packets.Enqueue(message);
    }

    public static void InitServerDatabase(string path, string overridePath, string lang)
    {
        ServerDatabase = new ServerDatabase(path, overridePath) { Language = Enum.Parse<LangEnum>(lang.Replace("EU-", "")) };
    }

    static void Init(CaptureMode mode, bool toolboxMode)
    {
        Factory = new MessageFactory();

        Sniffer = SnifferFactory.Create(mode, toolboxMode);
        Sniffer.NewConnection += OnNewConnection;
        Sniffer.EndConnection += OnEndConnection;
        Sniffer.MessageReceived += EnqueuePacket;
        Sniffer.Enabled = true;


        AnalysisThread = new Thread(PacketAnalysisLoop) { Name = "Analysis" };
        AnalysisThread.Start();
    }

    static void PacketAnalysisLoop()
    {
        Processor = new MessageProcessor();

        if (ProcessorReady != null) Utilities.GetMainDispatcher().InvokeAsync(ProcessorReady);
        while (true)
        {
            if (!Packets.TryDequeue(out var pkt))
            {
                Thread.Sleep(1);
                continue;
            }

            ParsedMessage? msg;
            try
            {
                msg = Factory!.Create(pkt);
            }
            catch (Exception ex)
            {
                var opcName = Factory!.OpCodeNamer.GetName(pkt.OpCode);
                throw new PacketParseException($"Failed to parse packet {opcName}", ex, opcName, pkt.Data.Array);
            }
            Processor.Handle(msg);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    static void OnNewConnection(Server srv)
    {
        if (Sniffer is not TeraSniffer) return;
        DetectClientVersionFromFile();
    }

    static void OnEndConnection()
    {
        Log.N("TCC", SR.Disconnected, NotificationType.None);
    }

    static void DetectClientVersionFromFile()
    {
        try
        {
            var process = Process.GetProcessesByName("TERA")[0];
            var fullPath = process.MainModule?.FileName.Replace("TERA.exe", "ReleaseRevision.txt");
            if (fullPath == null) throw new FileNotFoundException("TERA.exe not found.");

            var txt = File.ReadAllLines(fullPath);
            foreach (var line in txt)
            {
                if (!line.StartsWith("Version:")) continue;
                var idx = line.IndexOf("Live-", StringComparison.InvariantCultureIgnoreCase);
                var v = line[(idx + 5)..]; //Substring(idx + 5)
                var idx2 = v.IndexOf(' ');
                v = v[..idx2]; //Substring(0, idx2)
                v = v.Replace(".", "");
                Factory!.ReleaseVersion = int.Parse(v);
            }
        }
        catch (Exception e)
        {
            throw new ClientVersionDetectionException("Failed to detect client version from file", e);
        }
    }
}