using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_OTHER_USER_APPLY_PARTY : ParsedMessage
    {
        public uint PlayerId { get; set; }
        public Class Class { get; set; }
        public short Level { get; set; }
        public string Name { get; set; }

        public S_OTHER_USER_APPLY_PARTY(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            reader.Skip(1);
            PlayerId = reader.ReadUInt32();
            Class = (Class)reader.ReadInt16();
            reader.Skip(4);
            Level = reader.ReadInt16();
            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();
        }
    }
}
