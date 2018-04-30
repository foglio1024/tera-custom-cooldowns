using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_BAN_PARTY : ParsedMessage
    {
        internal S_BAN_PARTY(TeraMessageReader reader) : base(reader)
        {
        }
    }
}