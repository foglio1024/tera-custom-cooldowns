using TCC.ViewModels;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_SYSTEM_MESSAGE : ParsedMessage
    {
        public string Message { get; private set; }
        public S_SYSTEM_MESSAGE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2);
            Message = reader.ReadTeraString();
        }
    }
}
