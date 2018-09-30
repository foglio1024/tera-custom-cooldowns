using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CHAT : ParsedMessage
    {
        private ushort authorNameOffset, messageOffset;
        private uint ch;
        private ulong authorId;
        private byte unk1, gm, unk2;
        private string authorName;
        private string message;

        public ChatChannel Channel => (ChatChannel)ch;
        public ulong AuthorId => authorId;
        public string AuthorName => authorName;
        public string Message => message;

        public S_CHAT(TeraMessageReader reader) : base(reader)
        {
            authorNameOffset = reader.ReadUInt16();
            messageOffset = reader.ReadUInt16();
            ch = reader.ReadUInt32();
            authorId = reader.ReadUInt64();
            reader.Skip(3);
            authorName = reader.ReadTeraString();
            message = reader.ReadTeraString();

            if (ch == 212) ch = 26;
        }
    }
}
