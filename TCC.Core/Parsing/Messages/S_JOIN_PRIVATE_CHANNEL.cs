using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_JOIN_PRIVATE_CHANNEL : ParsedMessage
    {
        public int Index { get; private set; }
        public uint Id { get; private set; }
        public string Name { get; private set; }

        public S_JOIN_PRIVATE_CHANNEL(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            var nameOffset = reader.ReadUInt16();
            Index = reader.ReadInt32();
            Id = reader.ReadUInt32();
            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();
        }
    }
}
