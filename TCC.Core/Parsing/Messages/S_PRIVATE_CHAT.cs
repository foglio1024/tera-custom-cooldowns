using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PRIVATE_CHAT : ParsedMessage
    {
        ushort authorNameOffset, messageOffset;
        uint ch;
        ulong authorId;
        string authorName;
        string message;

        public uint Channel { get => ch; }
        public ulong AuthorId { get => authorId; }
        public string AuthorName { get => authorName; }
        public string Message { get => message; }

        public S_PRIVATE_CHAT(TeraMessageReader reader) : base(reader)
        {
            //authorNameOffset = reader.ReadUInt16();
            //messageOffset = reader.ReadUInt16();
            reader.Skip(4);
            ch = reader.ReadUInt32();
            authorId = reader.ReadUInt64();
            authorName = reader.ReadTeraString();
            message = reader.ReadTeraString();
        }
    }
}
