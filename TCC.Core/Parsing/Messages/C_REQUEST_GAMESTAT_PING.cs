using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class C_REQUEST_GAMESTAT_PING : ParsedMessage
    {
        public C_REQUEST_GAMESTAT_PING(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
