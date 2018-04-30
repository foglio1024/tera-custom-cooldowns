using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_RETURN_TO_LOBBY : ParsedMessage
    {
        public S_RETURN_TO_LOBBY(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
