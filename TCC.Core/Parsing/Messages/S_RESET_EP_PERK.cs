using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_RESET_EP_PERK : ParsedMessage
    {
        public bool Success { get; }
        public S_RESET_EP_PERK(TeraMessageReader r) : base(r)
        {
            Success = r.ReadBoolean();
        }
    }
}