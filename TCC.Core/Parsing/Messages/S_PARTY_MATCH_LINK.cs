using TCC.Data;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MATCH_LINK : ParsedMessage
    {
        public S_PARTY_MATCH_LINK(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            var msgOffset = reader.ReadUInt16();

            Id = reader.ReadInt32();
            reader.Skip(1);
            Raid = reader.ReadBoolean();

            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();

            reader.BaseStream.Position = msgOffset - 4;
            Message = ChatMessage.ReplaceEscapes(reader.ReadTeraString());
        }

        public int Id { get; private set; }
        public bool Raid { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
    }
}
