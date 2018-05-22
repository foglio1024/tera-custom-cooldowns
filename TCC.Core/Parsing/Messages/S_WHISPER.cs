using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_WHISPER : ParsedMessage
    {
        public ulong PlayerId { get; private set; }
        public string Author { get; private set; }
        public string Recipient { get; private set; }
        public string Message { get; private set; }
        public S_WHISPER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(6);
            PlayerId = reader.ReadUInt64();
            reader.Skip(3);
            Author = reader.ReadTeraString();
            Recipient = reader.ReadTeraString();
            Message = reader.ReadTeraString();
        }
    }
}
