using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SPAWN_ME : ParsedMessage
    {
        public S_SPAWN_ME(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
