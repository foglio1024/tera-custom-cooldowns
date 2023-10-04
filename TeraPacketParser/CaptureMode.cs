using System.ComponentModel;

namespace TeraPacketParser;

public enum CaptureMode
{
    [Description("npcap")]
    Npcap,
    [Description("Raw sockets")]
    RawSockets,
    [Description("TERA Toolbox")]
    Toolbox
}