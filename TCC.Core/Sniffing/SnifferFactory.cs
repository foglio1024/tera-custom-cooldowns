using TCC.Data;
using TeraPacketParser.TeraCommon.Sniffing;

namespace TCC.Sniffing
{
    public static class SnifferFactory
    {
        /// <summary>
        /// Creates a new <see cref="ITeraSniffer"/> based on current settings.
        /// </summary>
        /// <returns>a <see cref="TeraSniffer"/> or <see cref="ToolboxSniffer"/> based on <see cref="CaptureMode"/> and <see cref="App"/>.ToolboxMode</returns>
        public static ITeraSniffer Create()
        {
            return (App.Settings.CaptureMode, App.ToolboxMode) switch
            {
                (CaptureMode.Npcap,      false) => new TeraSniffer(true,  Game.DB.ServerDatabase.GetServersByIp()),
                (CaptureMode.RawSockets, false) => new TeraSniffer(false, Game.DB.ServerDatabase.GetServersByIp()),
                _ => new ToolboxSniffer()
            };
        }
    }
}
