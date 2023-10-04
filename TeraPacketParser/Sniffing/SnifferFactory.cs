using TeraPacketParser.Analysis;
using TeraPacketParser.TeraCommon.Sniffing;

namespace TeraPacketParser.Sniffing;

public static class SnifferFactory
{
    /// <summary>
    /// Creates a new <see cref="ITeraSniffer"/> based on current settings.
    /// </summary>
    /// <returns>a <see cref="TeraSniffer"/> or <see cref="ToolboxSniffer"/> based on <see cref="CaptureMode"/> and ToolboxMode</returns>
    public static ITeraSniffer Create(CaptureMode mode, bool toolboxMode)
    {
        return (mode, toolboxMode) switch
        {
            (CaptureMode.Npcap,      false) => new TeraSniffer(true,  PacketAnalyzer.ServerDatabase.GetServersByIp()),
            (CaptureMode.RawSockets, false) => new TeraSniffer(false, PacketAnalyzer.ServerDatabase.GetServersByIp()),
            _ => new ToolboxSniffer()
        };
    }
}