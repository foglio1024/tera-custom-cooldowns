using System;
using TCC.TeraCommon.Sniffing;
using TeraPacketParser;
using Server = TCC.TeraCommon.Game.Server;

namespace TCC.Parsing
{
    internal class ToolboxSniffer : ITeraSniffer
    {
        // TODO: start/stop TCP connection to TTB
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        // TODO: check on TCP connection to TTB
        public bool Connected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<Message> MessageReceived;
        public event Action<Server> NewConnection;
        public event Action EndConnection;
    }
}