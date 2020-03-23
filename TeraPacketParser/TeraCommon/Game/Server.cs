namespace TeraPacketParser.TeraCommon.Game
{
    public class Server
    {
        public Server(string name, string region, string ip, uint serverId = uint.MaxValue)
        {
            Ip = ip;
            Name = name;
            Region = region;
            ServerId = serverId;
        }

        public string Ip { get; private set; }
        public string Name { get; private set; }
        public string Region { get; private set; }
        public uint ServerId { get; private set; }

        public override string ToString()
        {
            return $"[{Region}/{ServerId}] {Name} @ {Ip}";
        }
    }
}