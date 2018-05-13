using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CHECK_TO_READY_PARTY_FIN : ParsedMessage
    {
        public S_CHECK_TO_READY_PARTY_FIN(TeraMessageReader reader) : base(reader)
        {
        }
    }
}
