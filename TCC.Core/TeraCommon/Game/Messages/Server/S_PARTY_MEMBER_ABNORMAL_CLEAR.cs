using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SPartyMemberAbnormalClear : ParsedMessage
    {
        internal SPartyMemberAbnormalClear(TeraMessageReader reader) : base(reader)
        {
            //  PrintRaw();
        }
    }
}