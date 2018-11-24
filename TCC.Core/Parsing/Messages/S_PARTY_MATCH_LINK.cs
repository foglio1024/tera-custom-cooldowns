using TCC.Data.Chat;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MATCH_LINK : ParsedMessage
    {
        public S_PARTY_MATCH_LINK(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            var msgOffset = reader.ReadUInt16();

            Id = reader.ReadUInt32();
            reader.Skip(1);
            Raid = reader.ReadBoolean();

            reader.BaseStream.Position = nameOffset - 4;
            Name = reader.ReadTeraString();

            reader.BaseStream.Position = msgOffset - 4;
            Message = StringUtils.ReplaceHtmlEscapes(reader.ReadTeraString());
        }

        public uint Id { get; private set; }
        public bool Raid { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
    }
}
