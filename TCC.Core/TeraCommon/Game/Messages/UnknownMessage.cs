using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages
{
    // Created when we want a parsed message, but don't know how to handle that OpCode
    public class UnknownMessage : ParsedMessage
    {
        internal UnknownMessage(TeraMessageReader reader)
            : base(reader)
        {
        }
    }
}