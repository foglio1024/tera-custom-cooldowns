using TCC.Annotations;
using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CHAT : ParsedMessage
    {
        public ChatChannel Channel { get; }
        public ulong AuthorId { [UsedImplicitly] get; }
        public string AuthorName { get; }
        public string Message { get; }

        public S_CHAT(TeraMessageReader reader) : base(reader)
        {
            var authorNameOffset = reader.ReadUInt16();
            var messageOffset = reader.ReadUInt16();
            var ch = reader.ReadUInt32();
            AuthorId = reader.ReadUInt64();
            reader.Skip(3);
            reader.BaseStream.Position = authorNameOffset - 4;
            AuthorName= reader.ReadTeraString();
            reader.BaseStream.Position = messageOffset - 4;
            Message = reader.ReadTeraString();
            Channel = ch == 212 ? (ChatChannel)26 : (ChatChannel)ch;
        }
    }
}
