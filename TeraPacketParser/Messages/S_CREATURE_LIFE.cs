


namespace TeraPacketParser.Messages
{
    public class S_CREATURE_LIFE : ParsedMessage
    {
        public ulong Target { get; }
        public bool Alive { get; }

        internal S_CREATURE_LIFE(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            //var pos = reader.ReadVector3f();
            reader.Skip(12);
            Alive = reader.ReadBoolean(); // 0=dead;1=alive
        }
    }
}