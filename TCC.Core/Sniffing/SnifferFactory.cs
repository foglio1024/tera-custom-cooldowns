using TCC.Data;
using TCC.Parsing;
using TCC.TeraCommon.Sniffing;

namespace TCC.Sniffing
{
    public static class SnifferFactory
    {
        public static ITeraSniffer Create()
        {
            switch (App.Settings.CaptureMode)
            {
                case CaptureMode.Npcap when !App.ToolboxMode:
                case CaptureMode.RawSockets when !App.ToolboxMode:
                    return new TeraSniffer();
                default:
                    return new ToolboxSniffer();
            }
        }
    }
}
