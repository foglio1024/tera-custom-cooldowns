


namespace TeraPacketParser.Messages
{
    public class C_PLAYER_FLYING_LOCATION : ParsedMessage
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public C_PLAYER_FLYING_LOCATION(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }
}
