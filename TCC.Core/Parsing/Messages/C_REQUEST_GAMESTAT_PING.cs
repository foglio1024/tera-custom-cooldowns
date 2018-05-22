using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class C_REQUEST_GAMESTAT_PING : ParsedMessage
    {
        public C_REQUEST_GAMESTAT_PING(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
