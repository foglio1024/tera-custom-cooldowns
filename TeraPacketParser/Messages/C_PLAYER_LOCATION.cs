


namespace TeraPacketParser.Messages
{
    public class C_PLAYER_LOCATION : ParsedMessage
    {
        public float X { get; }
        public float Y { get; }

        public C_PLAYER_LOCATION(TeraMessageReader reader) : base(reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }
    }
}
