using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_LEAVE_PARTY : ParsedMessage
    {
        internal S_LEAVE_PARTY(TeraMessageReader reader) : base(reader)
        {
        }
    }
}