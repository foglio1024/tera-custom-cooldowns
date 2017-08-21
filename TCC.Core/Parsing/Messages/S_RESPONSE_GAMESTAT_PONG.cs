using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_RESPONSE_GAMESTAT_PONG : ParsedMessage
    {
        public S_RESPONSE_GAMESTAT_PONG(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
