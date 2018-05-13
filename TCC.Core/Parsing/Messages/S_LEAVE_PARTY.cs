using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LEAVE_PARTY : ParsedMessage
    {
        public S_LEAVE_PARTY(TeraMessageReader reader) : base(reader)
        {

        }
    }
}